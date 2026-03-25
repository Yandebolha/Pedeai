using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class PedidoDAL : BaseDAL
    {
        /// <summary>
        /// Lista pedidos com filtros opcionais de situação e data.
        /// Retorna DataTable pronto para binding no DataGridView.
        /// </summary>
        public DataTable Listar(string situacao = null, DateTime? data = null)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();

            var sql = @"SELECT p.Codigo,
                               p.pediNumero            AS Numero,
                               p.pediNome_Cliente      AS Cliente,
                               p.pediTelefone_Cliente  AS Telefone,
                               p.pediSituacao          AS Status,
                               p.pediForma_Pagamento   AS Pagamento,
                               p.pediTipo_Entrega      AS Entrega,
                               p.pediValor_Total       AS Total,
                               p.pediOrigem            AS Origem,
                               p.pediData_Lancamento   AS DataHora
                        FROM pedido_web p
                        WHERE 1=1";

            if (!string.IsNullOrEmpty(situacao)) sql += " AND p.pediSituacao = @sit";
            if (data.HasValue) sql += " AND DATE(p.pediData_Lancamento) = @data";
            sql += " ORDER BY p.pediData_Lancamento DESC LIMIT 200";

            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrEmpty(situacao)) cmd.Parameters.AddWithValue("@sit", situacao);
            if (data.HasValue) cmd.Parameters.AddWithValue("@data", data.Value.Date);

            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Busca um pedido completo pelo Codigo.</summary>
        public PedidoWeb PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM pedido_web WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapearPedido(r);
        }

        /// <summary>Lista os itens de um pedido.</summary>
        public DataTable ListarItens(int codigoPedido)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT i.itpwNome_Mercadoria AS Produto,
                               i.itpwQtde            AS Qtde,
                               i.itpwPreco_Unitario  AS Unitario,
                               i.itpwSubtotal        AS Subtotal,
                               i.itpwObservacoes     AS Obs
                        FROM itens_pedido_web i
                        WHERE i.Codigo_Pedido = @cod
                        ORDER BY i.Codigo";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cod", codigoPedido);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Insere pedido manual com seus itens em transação única.</summary>
        public string InserirManual(PedidoWeb pedido, System.Collections.Generic.List<ItemPedidoWeb> itens)
        {
            try
            {
                using var conn = AbrirConexao();
                using var trans = conn.BeginTransaction();

                pedido.Codigo    = ProximoCodigo("pedido_web", conn, trans);
                pedido.auxCodigo = ProximoAuxCodigo("pedido_web", conn, trans);
                pedido.pediData_Lancamento = DateTime.Now;

                // Gera número sequencial simples: MAN + data + codigo
                pedido.pediNumero = "MAN" + DateTime.Now.ToString("yyyyMMdd") + pedido.Codigo.ToString("D4");

                var sqlP = @"INSERT INTO pedido_web
                    (auxCodigo, Codigo, pediNumero, pediNome_Cliente, pediTelefone_Cliente,
                     pediSituacao, pediTipo_Entrega, pediForma_Pagamento, pediOrigem,
                     pediSubtotal, pediTaxa_Entrega, pediDesconto, pediValor_Total, pediTroco_Para,
                     pediEndereco_Entrega, pediObservacoes, pediData_Lancamento, Situacao, Info)
                    VALUES(@aux,@cod,@num,@nome,@tel,@sit,@tent,@fpag,3,
                           @sub,@taxa,0,@total,@troco,@end,@obs,@dt,'A','')";
                using var cmdP = new MySqlCommand(sqlP, conn, trans);
                cmdP.Parameters.AddWithValue("@aux",   pedido.auxCodigo);
                cmdP.Parameters.AddWithValue("@cod",   pedido.Codigo);
                cmdP.Parameters.AddWithValue("@num",   pedido.pediNumero);
                cmdP.Parameters.AddWithValue("@nome",  pedido.pediNome_Cliente ?? "");
                cmdP.Parameters.AddWithValue("@tel",   pedido.pediTelefone_Cliente ?? "");
                cmdP.Parameters.AddWithValue("@sit",   0); // Pendente
                cmdP.Parameters.AddWithValue("@tent",  pedido.pediTipo_Entrega);
                cmdP.Parameters.AddWithValue("@fpag",  pedido.pediForma_Pagamento);
                cmdP.Parameters.AddWithValue("@sub",   pedido.pediSubtotal);
                cmdP.Parameters.AddWithValue("@taxa",  pedido.pediTaxa_Entrega);
                cmdP.Parameters.AddWithValue("@total", pedido.pediValor_Total);
                cmdP.Parameters.AddWithValue("@troco", pedido.pediTroco_Para ?? (object)DBNull.Value);
                cmdP.Parameters.AddWithValue("@end",   pedido.pediEndereco_Entrega ?? "");
                cmdP.Parameters.AddWithValue("@obs",   pedido.pediObservacoes ?? "");
                cmdP.Parameters.AddWithValue("@dt",    pedido.pediData_Lancamento);
                cmdP.ExecuteNonQuery();

                // Itens
                foreach (var item in itens)
                {
                    item.Codigo       = ProximoCodigo("itens_pedido_web", conn, trans);
                    item.auxCodigo    = ProximoAuxCodigo("itens_pedido_web", conn, trans);
                    item.Codigo_Pedido = pedido.Codigo;

                    var sqlI = @"INSERT INTO itens_pedido_web
                        (auxCodigo,Codigo,Codigo_Pedido,Codigo_Mercadoria,
                         itpwNome_Mercadoria,itpwQtde,itpwPreco_Unitario,itpwSubtotal,itpwObservacoes,Situacao)
                        VALUES(@aux,@cod,@pedido,@merc,@nome,@qtde,@unit,@sub,@obs,'A')";
                    using var cmdI = new MySqlCommand(sqlI, conn, trans);
                    cmdI.Parameters.AddWithValue("@aux",    item.auxCodigo);
                    cmdI.Parameters.AddWithValue("@cod",    item.Codigo);
                    cmdI.Parameters.AddWithValue("@pedido", item.Codigo_Pedido);
                    cmdI.Parameters.AddWithValue("@merc",   item.Codigo_Mercadoria);
                    cmdI.Parameters.AddWithValue("@nome",   item.itpwNome_Mercadoria ?? "");
                    cmdI.Parameters.AddWithValue("@qtde",   item.itpwQtde);
                    cmdI.Parameters.AddWithValue("@unit",   item.itpwPreco_Unitario);
                    cmdI.Parameters.AddWithValue("@sub",    item.itpwSubtotal);
                    cmdI.Parameters.AddWithValue("@obs",    item.itpwObservacoes ?? "");
                    cmdI.ExecuteNonQuery();
                }

                trans.Commit();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Atualiza a situação de um pedido.</summary>
        public void AtualizarSituacao(int codigo, int novaSituacao)
        {
            using var conn = AbrirConexao();
            var sql = "UPDATE pedido_web SET pediSituacao = @sit, pediData_Atualizacao = NOW() WHERE Codigo = @cod";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sit", novaSituacao);
            cmd.Parameters.AddWithValue("@cod", codigo);
            cmd.ExecuteNonQuery();
        }

        private static PedidoWeb MapearPedido(MySqlDataReader r)
        {
            return new PedidoWeb
            {
                Codigo                = Convert.ToInt32(r["Codigo"]),
                auxCodigo             = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
                pediNumero            = r["pediNumero"]?.ToString() ?? "",
                pediNome_Cliente      = r["pediNome_Cliente"]?.ToString() ?? "",
                pediTelefone_Cliente  = r["pediTelefone_Cliente"]?.ToString() ?? "",
                pediSituacao          = Convert.ToInt32(r["pediSituacao"]),
                pediForma_Pagamento   = r["pediForma_Pagamento"] == DBNull.Value ? 0 : Convert.ToInt32(r["pediForma_Pagamento"]),
                pediTipo_Entrega      = r["pediTipo_Entrega"] == DBNull.Value ? 0 : Convert.ToInt32(r["pediTipo_Entrega"]),
                pediValor_Total       = Convert.ToDecimal(r["pediValor_Total"]),
                pediOrigem            = r["pediOrigem"] == DBNull.Value ? 0 : Convert.ToInt32(r["pediOrigem"]),
                pediData_Lancamento   = Convert.ToDateTime(r["pediData_Lancamento"]),
                Info                  = r["Info"]?.ToString() ?? "",
            };
        }
    }
}

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
                pediForma_Pagamento   = r["pediForma_Pagamento"]?.ToString() ?? "",
                pediTipo_Entrega      = r["pediTipo_Entrega"]?.ToString() ?? "",
                pediValor_Total       = Convert.ToDecimal(r["pediValor_Total"]),
                pediOrigem            = r["pediOrigem"]?.ToString() ?? "",
                pediData_Lancamento   = Convert.ToDateTime(r["pediData_Lancamento"]),
                Status_Transmissao    = r["Status_Transmissao"]?.ToString() ?? "N",
                Info                  = r["Info"]?.ToString() ?? "",
            };
        }
    }
}

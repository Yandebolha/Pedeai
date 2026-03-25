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
        public DataTable Listar(string filtro = null, DateTime? data = null)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();

            var sql = @"SELECT p.Codigo,
                               p.pediNumero                AS Numero,
                               p.pediNome_Cliente          AS Cliente,
                               p.pediTelefone_Cliente      AS Telefone,
                               CASE p.pediSituacao
                                   WHEN 0 THEN 'Pendente'
                                   WHEN 1 THEN 'Confirmado'
                                   WHEN 2 THEN 'Em Preparo'
                                   WHEN 3 THEN 'Pronto'
                                   WHEN 4 THEN 'Saiu p/ Entrega'
                                   WHEN 5 THEN 'Entregue'
                                   WHEN 6 THEN 'Cancelado'
                                   ELSE CAST(p.pediSituacao AS CHAR)
                               END                          AS Status,
                               (SELECT COUNT(*) FROM itens_pedido_web i WHERE i.Codigo_Pedido = p.Codigo) AS Itens,
                               CASE p.pediForma_Pagamento
                                   WHEN 0 THEN 'Dinheiro'
                                   WHEN 1 THEN 'Cartão'
                                   WHEN 2 THEN 'Pix'
                                   ELSE CAST(p.pediForma_Pagamento AS CHAR)
                               END                          AS Pagamento,
                               CASE p.pediTipo_Entrega
                                   WHEN 0 THEN 'Retirada'
                                   WHEN 1 THEN 'Entrega'
                                   ELSE CAST(p.pediTipo_Entrega AS CHAR)
                               END                          AS Entrega,
                               p.pediValor_Total           AS Total,
                               p.pediOrigem                AS Origem,
                               p.pediData_Lancamento       AS DataHora
                        FROM pedido_web p
                        WHERE 1=1";

            if      (filtro == "emPreparo")  sql += " AND p.pediSituacao = 2";
            else if (filtro == "finalizados") sql += " AND p.pediSituacao IN (3,4,5)";
            else if (filtro == "cancelados")  sql += " AND p.pediSituacao = 6";

            if (data.HasValue) sql += " AND DATE(p.pediData_Lancamento) = @data";
            sql += " ORDER BY p.pediData_Lancamento DESC LIMIT 200";

            using var cmd = new MySqlCommand(sql, conn);
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

                // Número sequencial diário: ex. 001, 002… reinicia a cada dia
                int seqDia;
                using (var cmdSeq = new MySqlCommand(
                    "SELECT COALESCE(MAX(CAST(SUBSTR(pediNumero,9) AS UNSIGNED)),0)+1 " +
                    "FROM pedido_web WHERE DATE(pediData_Lancamento)=CURDATE()", conn, trans))
                {
                    var r2 = cmdSeq.ExecuteScalar();
                    seqDia = r2 == null || r2 == DBNull.Value ? 1 : Convert.ToInt32(r2);
                }
                pedido.pediNumero = DateTime.Now.ToString("yyyyMMdd") + seqDia.ToString("D3");

                var sqlP = @"INSERT INTO pedido_web
                    (auxCodigo, Codigo, pediNumero, pediNome_Cliente, pediTelefone_Cliente,
                     pediSituacao, pediTipo_Entrega, pediForma_Pagamento, pediOrigem,
                     pediSubtotal, pediTaxa_Entrega, pediDesconto, pediValor_Total, pediTroco_Para,
                     pediEndereco_Entrega, pediObservacoes, pediData_Lancamento, Situacao, Info, Codigo_Cliente)
                    VALUES(@aux,@cod,@num,@nome,@tel,@sit,@tent,@fpag,3,
                           @sub,@taxa,0,@total,@troco,@end,@obs,@dt,'A','',@cliCod)";
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
                cmdP.Parameters.AddWithValue("@cliCod", pedido.Codigo_Cliente > 0 ? (object)pedido.Codigo_Cliente : DBNull.Value);
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

                    // Verificar estoque disponível antes da baixa
                    var sqlChkEst = @"SELECT mercMercadoria, mercEstoque_Atual, mercControla_Estoque
                                      FROM mercadoria WHERE Codigo = @merc LIMIT 1";
                    using var cmdChk = new MySqlCommand(sqlChkEst, conn, trans);
                    cmdChk.Parameters.AddWithValue("@merc", item.Codigo_Mercadoria);
                    using (var rdr = cmdChk.ExecuteReader())
                    {
                        if (rdr.Read() && Convert.ToInt32(rdr["mercControla_Estoque"]) == 1)
                        {
                            var nomeProd   = rdr["mercMercadoria"]?.ToString() ?? item.itpwNome_Mercadoria;
                            var estDisp    = rdr["mercEstoque_Atual"] == DBNull.Value
                                                ? 0m
                                                : Convert.ToDecimal(rdr["mercEstoque_Atual"]);
                            if (item.itpwQtde > estDisp)
                            {
                                trans.Rollback();
                                return $"Estoque insuficiente para \"{nomeProd}\": " +
                                       $"disponível {estDisp:0.##}, solicitado {item.itpwQtde:0.##}.";
                            }
                        }
                    }

                    // Baixa de estoque (apenas quando o produto controla estoque)
                    var sqlEst = @"UPDATE mercadoria
                                   SET mercEstoque_Atual = mercEstoque_Atual - @qtde
                                   WHERE Codigo = @merc AND mercControla_Estoque = 1";
                    using var cmdEst = new MySqlCommand(sqlEst, conn, trans);
                    cmdEst.Parameters.AddWithValue("@qtde", item.itpwQtde);
                    cmdEst.Parameters.AddWithValue("@merc", item.Codigo_Mercadoria);
                    cmdEst.ExecuteNonQuery();
                }

                trans.Commit();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Atualiza a situacao de um pedido. Para cancelamento (sit=6), informe canceladoPor.</summary>
        public void AtualizarSituacao(int codigo, int novaSituacao, string canceladoPor = null)
        {
            using var conn = AbrirConexao();
            using var trans = conn.BeginTransaction();

            string sql;
            if (novaSituacao == 6 && canceladoPor != null)
                sql = "UPDATE pedido_web SET pediSituacao=@sit, pediCancelado_Por=@cpor, pediData_Atualizacao=NOW() WHERE Codigo=@cod";
            else
                sql = "UPDATE pedido_web SET pediSituacao=@sit, pediData_Atualizacao=NOW() WHERE Codigo=@cod";
            using var cmd = new MySqlCommand(sql, conn, trans);
            cmd.Parameters.AddWithValue("@sit", novaSituacao);
            if (novaSituacao == 6 && canceladoPor != null)
                cmd.Parameters.AddWithValue("@cpor", canceladoPor);
            cmd.Parameters.AddWithValue("@cod", codigo);
            cmd.ExecuteNonQuery();

            // Ao cancelar (situação 6), devolver estoque dos itens do pedido
            if (novaSituacao == 6)
            {
                var sqlRestaurar = @"UPDATE mercadoria m
                                     JOIN itens_pedido_web i ON i.Codigo_Mercadoria = m.Codigo
                                     SET m.mercEstoque_Atual = m.mercEstoque_Atual + i.itpwQtde
                                     WHERE i.Codigo_Pedido = @cod
                                       AND m.mercControla_Estoque = 1";
                using var cmdRest = new MySqlCommand(sqlRestaurar, conn, trans);
                cmdRest.Parameters.AddWithValue("@cod", codigo);
                cmdRest.ExecuteNonQuery();
            }

            trans.Commit();
        }

        /// <summary>Finaliza pedido gravando valor pago e código de transação.</summary>
        public void FinalizarPedido(int codigo, int novaSituacao, decimal valorPago, string transacao)
        {
            using var conn = AbrirConexao();
            var sql = @"UPDATE pedido_web
                        SET pediSituacao          = @sit,
                            pediValor_Pago        = @pago,
                            pediCodigo_Transacao  = @trans,
                            pediData_Atualizacao  = NOW()
                        WHERE Codigo = @cod";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sit",   novaSituacao);
            cmd.Parameters.AddWithValue("@pago",  valorPago);
            cmd.Parameters.AddWithValue("@trans", transacao ?? "");
            cmd.Parameters.AddWithValue("@cod",   codigo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>Retorna resumo financeiro de um periodo.</summary>
        public DataTable GetFinanceiro(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT
                DATE(p.pediData_Lancamento)         AS Dia,
                COUNT(*)                            AS Pedidos,
                SUM(p.pediSubtotal)                 AS Subtotal,
                SUM(p.pediTaxa_Entrega)             AS TaxaEntrega,
                SUM(p.pediDesconto)                 AS Descontos,
                SUM(p.pediValor_Total)              AS TotalBruto,
                SUM(CASE WHEN p.pediTipo_Entrega=1 THEN p.pediValor_Total ELSE 0 END) AS ValorEntrega,
                SUM(CASE WHEN p.pediTipo_Entrega=0 THEN p.pediValor_Total ELSE 0 END) AS ValorRetirada,
                SUM(CASE WHEN p.pediForma_Pagamento=0 THEN p.pediValor_Total ELSE 0 END) AS Dinheiro,
                SUM(CASE WHEN p.pediForma_Pagamento=1 THEN p.pediValor_Total ELSE 0 END) AS Cartao,
                SUM(CASE WHEN p.pediForma_Pagamento=2 THEN p.pediValor_Total ELSE 0 END) AS Pix,
                COALESCE((
                    SELECT SUM(i.itpwQtde * COALESCE(m.mercPreco_Custo, 0))
                    FROM itens_pedido_web i
                    LEFT JOIN mercadoria m ON m.Codigo = i.Codigo_Mercadoria
                    WHERE i.Codigo_Pedido = p.Codigo
                ), 0) AS CustoMercadorias
              FROM pedido_web p
              WHERE DATE(p.pediData_Lancamento) BETWEEN @de AND @ate
                AND p.pediSituacao NOT IN (6)
              GROUP BY DATE(p.pediData_Lancamento)
              ORDER BY Dia DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
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
                pediCancelado_Por     = r["pediCancelado_Por"] == DBNull.Value ? null : r["pediCancelado_Por"]?.ToString(),
                Info                  = r["Info"]?.ToString() ?? "",
            };
        }
    }
}

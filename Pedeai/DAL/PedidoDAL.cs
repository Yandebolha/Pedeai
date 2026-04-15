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
                               COALESCE(
                                   NULLIF(CONCAT_WS(' + ',
                                       IF(COALESCE(p.pediPago_Dinheiro,0)>0,'Dinheiro',NULL),
                                       IF(COALESCE(p.pediPago_Cartao  ,0)>0,'Cartão'  ,NULL),
                                       IF(COALESCE(p.pediPago_Pix     ,0)>0,'Pix'     ,NULL)
                                   ),''),
                                   CASE p.pediForma_Pagamento
                                       WHEN 0 THEN 'Dinheiro'
                                       WHEN 1 THEN 'Cartão'
                                       WHEN 2 THEN 'Pix'
                                       ELSE 'Outro'
                                   END
                               )                            AS Pagamento,
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
            else if (filtro == "pendentes")   sql += " AND p.pediSituacao = 0";
            else if (filtro == "finalizados") sql += " AND p.pediSituacao IN (3,4,5)";
            else if (filtro == "cancelados")  sql += " AND p.pediSituacao = 6";

            if (data.HasValue) sql += " AND DATE(p.pediData_Lancamento) = @data";
            sql += " ORDER BY p.pediData_Lancamento DESC LIMIT 200";

            using var cmd = new MySqlCommand(sql, conn);
            if (data.HasValue) cmd.Parameters.AddWithValue("@data", data.Value.Date);

            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>
        /// Pesquisa pedidos por intervalo de datas, nome de cliente e/ou número — para a tela Consultar Pedido.
        /// </summary>
        public DataTable ListarConsulta(DateTime de, DateTime ate, string cliente = "", string numero = "")
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
                               p.pediValor_Total            AS Total,
                               p.pediData_Lancamento        AS DataHora
                        FROM pedido_web p
                        WHERE DATE(p.pediData_Lancamento) BETWEEN @de AND @ate";
            if (!string.IsNullOrWhiteSpace(cliente))
                sql += " AND p.pediNome_Cliente LIKE @cli";
            if (!string.IsNullOrWhiteSpace(numero))
                sql += " AND p.pediNumero = @num";
            sql += " ORDER BY p.pediData_Lancamento DESC LIMIT 500";

            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            if (!string.IsNullOrWhiteSpace(cliente)) cmd.Parameters.AddWithValue("@cli", $"%{cliente}%");
            if (!string.IsNullOrWhiteSpace(numero))  cmd.Parameters.AddWithValue("@num", numero.Trim());
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

        /// <summary>Lista os itens de um pedido como objetos (para impressao).</summary>
        public System.Collections.Generic.List<ItemPedidoWeb> ListarItensObjetos(int codigoPedido)
        {
            var lista = new System.Collections.Generic.List<ItemPedidoWeb>();
            using var conn = AbrirConexao();
            var sql = @"SELECT * FROM itens_pedido_web WHERE Codigo_Pedido = @cod ORDER BY Codigo";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cod", codigoPedido);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                lista.Add(new ItemPedidoWeb
                {
                    Codigo              = Convert.ToInt32(r["Codigo"]),
                    auxCodigo           = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
                    Codigo_Pedido       = Convert.ToInt32(r["Codigo_Pedido"]),
                    Codigo_Mercadoria   = r["Codigo_Mercadoria"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Mercadoria"]),
                    itpwNome_Mercadoria = r["itpwNome_Mercadoria"]?.ToString() ?? "",
                    itpwQtde            = Convert.ToDecimal(r["itpwQtde"]),
                    itpwPreco_Unitario  = Convert.ToDecimal(r["itpwPreco_Unitario"]),
                    itpwDesconto_Pct    = r["itpwDesconto_Pct"] == DBNull.Value ? 0m : Convert.ToDecimal(r["itpwDesconto_Pct"]),
                    itpwSubtotal        = Convert.ToDecimal(r["itpwSubtotal"]),
                    itpwObservacoes     = r["itpwObservacoes"]?.ToString() ?? "",
                });
            }
            return lista;
        }

        /// <summary>Lista os itens de um pedido.</summary>
        public DataTable ListarItens(int codigoPedido)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT i.itpwNome_Mercadoria AS Produto,
                               i.itpwQtde            AS Qtde,
                               i.itpwPreco_Unitario  AS Unitario,
                               i.itpwDesconto_Pct    AS Desconto,
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
                     pediEndereco_Entrega, pediObservacoes, pediData_Lancamento, Situacao, Info,
                     Codigo_Cliente, pediCodigo_Cupom)
                    VALUES(@aux,@cod,@num,@nome,@tel,@sit,@tent,@fpag,3,
                           @sub,@taxa,@desc,@total,@troco,@end,@obs,@dt,'A','',@cliCod,@cupomCod)";
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
                cmdP.Parameters.AddWithValue("@desc",  pedido.pediDesconto);
                cmdP.Parameters.AddWithValue("@total", pedido.pediValor_Total);
                cmdP.Parameters.AddWithValue("@troco", pedido.pediTroco_Para ?? (object)DBNull.Value);
                cmdP.Parameters.AddWithValue("@end",   pedido.pediEndereco_Entrega ?? "");
                cmdP.Parameters.AddWithValue("@obs",   pedido.pediObservacoes ?? "");
                cmdP.Parameters.AddWithValue("@dt",    pedido.pediData_Lancamento);
                cmdP.Parameters.AddWithValue("@cliCod", pedido.Codigo_Cliente > 0 ? (object)pedido.Codigo_Cliente : DBNull.Value);
                cmdP.Parameters.AddWithValue("@cupomCod", pedido.pediCodigo_Cupom ?? "");
                cmdP.ExecuteNonQuery();

                // Itens
                foreach (var item in itens)
                {
                    item.Codigo       = ProximoCodigo("itens_pedido_web", conn, trans);
                    item.auxCodigo    = ProximoAuxCodigo("itens_pedido_web", conn, trans);
                    item.Codigo_Pedido = pedido.Codigo;

                    var sqlI = @"INSERT INTO itens_pedido_web
                        (auxCodigo,Codigo,Codigo_Pedido,Codigo_Mercadoria,
                         itpwNome_Mercadoria,itpwQtde,itpwPreco_Unitario,itpwDesconto_Pct,itpwSubtotal,itpwObservacoes,Situacao)
                        VALUES(@aux,@cod,@pedido,@merc,@nome,@qtde,@unit,@desc_pct,@sub,@obs,'A')";
                    using var cmdI = new MySqlCommand(sqlI, conn, trans);
                    cmdI.Parameters.AddWithValue("@aux",    item.auxCodigo);
                    cmdI.Parameters.AddWithValue("@cod",    item.Codigo);
                    cmdI.Parameters.AddWithValue("@pedido", item.Codigo_Pedido);
                    cmdI.Parameters.AddWithValue("@merc",   item.Codigo_Mercadoria);
                    cmdI.Parameters.AddWithValue("@nome",   item.itpwNome_Mercadoria ?? "");
                    cmdI.Parameters.AddWithValue("@qtde",   item.itpwQtde);
                    cmdI.Parameters.AddWithValue("@unit",   item.itpwPreco_Unitario);
                    cmdI.Parameters.AddWithValue("@desc_pct", item.itpwDesconto_Pct);
                    cmdI.Parameters.AddWithValue("@sub",    item.itpwSubtotal);
                    cmdI.Parameters.AddWithValue("@obs",    item.itpwObservacoes ?? "");
                    cmdI.ExecuteNonQuery();

                    // Verificar estoque disponível antes da baixa
                    var sqlChkEst = @"SELECT mercMercadoria, mercEstoque_Atual, mercControla_Estoque
                                      FROM mercadoria WHERE Codigo = @merc LIMIT 1";
                    // Verificar estoque: ler dados para fora do reader antes de qualquer rollback,
                    // pois MySqlConnector não permite operações na conexão enquanto um reader está aberto.
                    string nomeProdInsuf = null;
                    decimal estDispInsuf = 0m;
                    using (var cmdChk = new MySqlCommand(sqlChkEst, conn, trans))
                    {
                        cmdChk.Parameters.AddWithValue("@merc", item.Codigo_Mercadoria);
                        using (var rdr = cmdChk.ExecuteReader())
                        {
                            if (rdr.Read() && Convert.ToInt32(rdr["mercControla_Estoque"]) == 1)
                            {
                                var nomeRdr = rdr["mercMercadoria"]?.ToString() ?? item.itpwNome_Mercadoria;
                                var estRdr  = rdr["mercEstoque_Atual"] == DBNull.Value
                                                 ? 0m
                                                 : Convert.ToDecimal(rdr["mercEstoque_Atual"]);
                                if (item.itpwQtde > estRdr)
                                {
                                    nomeProdInsuf = nomeRdr;
                                    estDispInsuf  = estRdr;
                                }
                            }
                        } // reader fechado aqui
                    } // cmdChk descartado aqui
                    if (nomeProdInsuf != null)
                    {
                        trans.Rollback();
                        return $"Estoque insuficiente para \"{nomeProdInsuf}\": " +
                               $"disponível {estDispInsuf:0.##}, solicitado {item.itpwQtde:0.##}.";
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

                // Increment coupon usage counter atomically if a coupon was applied
                if (!string.IsNullOrWhiteSpace(pedido.pediCodigo_Cupom))
                {
                    using var connCup = AbrirConexao();
                    using var cmdCup = new MySqlCommand(
                        "UPDATE cupom SET cupomUsos_Realizados = cupomUsos_Realizados + 1 WHERE cupomCodigo = @c",
                        connCup);
                    cmdCup.Parameters.AddWithValue("@c", pedido.pediCodigo_Cupom);
                    cmdCup.ExecuteNonQuery();
                }

                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Atualiza a situacao de um pedido. Para cancelamento (sit=6), informe canceladoPor.</summary>
        public void AtualizarSituacao(int codigo, int novaSituacao, string canceladoPor = null)
        {
            using var conn = AbrirConexao();
            using var trans = conn.BeginTransaction();

            // When cancelling (situação 6), decrement coupon usage if one was applied
            if (novaSituacao == 6)
            {
                string cupomCod = null;
                using (var cmdGC = new MySqlCommand(
                    "SELECT pediCodigo_Cupom FROM pedido_web WHERE Codigo=@cod LIMIT 1", conn, trans))
                {
                    cmdGC.Parameters.AddWithValue("@cod", codigo);
                    var r = cmdGC.ExecuteScalar();
                    cupomCod = r == DBNull.Value ? null : r?.ToString();
                }
                if (!string.IsNullOrWhiteSpace(cupomCod))
                {
                    using var cmdDec = new MySqlCommand(
                        "UPDATE cupom SET cupomUsos_Realizados = GREATEST(0, cupomUsos_Realizados - 1) WHERE cupomCodigo = @c",
                        conn, trans);
                    cmdDec.Parameters.AddWithValue("@c", cupomCod);
                    cmdDec.ExecuteNonQuery();
                }
            }

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
        public void FinalizarPedido(int codigo, int novaSituacao, decimal valorPago, string transacao,
                                    decimal pagoDinheiro = 0, decimal pagoCartao = 0, decimal pagoPix = 0,
                                    string autorizador = "")
        {
            using var conn = AbrirConexao();
            var sql = @"UPDATE pedido_web
                        SET pediSituacao          = @sit,
                            pediValor_Pago        = @pago,
                            pediDesconto          = CASE WHEN @pago < pediValor_Total THEN pediValor_Total - @pago ELSE pediDesconto END,
                            pediCodigo_Transacao  = @trans,
                            pediPago_Dinheiro     = @din,
                            pediPago_Cartao       = @car,
                            pediPago_Pix          = @pix,
                            pediAutorizador       = @aut,
                            pediData_Atualizacao  = NOW()
                        WHERE Codigo = @cod";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@sit",   novaSituacao);
            cmd.Parameters.AddWithValue("@pago",  valorPago);
            cmd.Parameters.AddWithValue("@trans", transacao ?? "");
            cmd.Parameters.AddWithValue("@din",   pagoDinheiro);
            cmd.Parameters.AddWithValue("@car",   pagoCartao);
            cmd.Parameters.AddWithValue("@pix",   pagoPix);
            cmd.Parameters.AddWithValue("@aut",   string.IsNullOrEmpty(autorizador) ? (object)DBNull.Value : autorizador);
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
                SUM(COALESCE(p.pediValor_Pago, p.pediValor_Total)) AS TotalBruto,
                SUM(CASE WHEN p.pediTipo_Entrega=1 THEN COALESCE(p.pediValor_Pago, p.pediValor_Total) ELSE 0 END) AS ValorEntrega,
                SUM(CASE WHEN p.pediTipo_Entrega=0 THEN COALESCE(p.pediValor_Pago, p.pediValor_Total) ELSE 0 END) AS ValorRetirada,
                SUM(COALESCE(p.pediPago_Dinheiro, 0)) AS Dinheiro,
                SUM(COALESCE(p.pediPago_Cartao,   0)) AS Cartao,
                SUM(COALESCE(p.pediPago_Pix,      0)) AS Pix,
                COALESCE(SUM(custo.CustoMerc), 0)   AS CustoMercadorias
              FROM pedido_web p
              LEFT JOIN (
                  SELECT i.Codigo_Pedido,
                         SUM(i.itpwQtde * COALESCE(m.mercPreco_Custo, 0)) AS CustoMerc
                  FROM itens_pedido_web i
                  LEFT JOIN mercadoria m ON m.Codigo = i.Codigo_Mercadoria
                  GROUP BY i.Codigo_Pedido
              ) custo ON custo.Codigo_Pedido = p.Codigo
              WHERE DATE(p.pediData_Lancamento) BETWEEN @de AND @ate
                AND p.pediSituacao NOT IN (6)
              GROUP BY DATE(p.pediData_Lancamento)
              ORDER BY Dia DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            dt.Columns.Add("TotalLiquido", typeof(decimal));
            foreach (DataRow row in dt.Rows)
            {
                var bruto  = row["TotalBruto"]         == DBNull.Value ? 0m : Convert.ToDecimal(row["TotalBruto"]);
                var custo  = row["CustoMercadorias"]   == DBNull.Value ? 0m : Convert.ToDecimal(row["CustoMercadorias"]);
                var taxa   = row["TaxaEntrega"]         == DBNull.Value ? 0m : Convert.ToDecimal(row["TaxaEntrega"]);
                row["TotalLiquido"] = bruto - custo - taxa;
            }
            return dt;
        }

        /// <summary>Retorna total de compras (entradas de mercadoria) por dia no período.</summary>
        public DataTable GetComprasPorDia(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT
                DATE(e.entData)       AS Dia,
                COUNT(*)              AS NumEntradas,
                SUM(e.entValorTotal)  AS TotalCompras
              FROM entrada_mercadoria e
              WHERE DATE(e.entData) BETWEEN @de AND @ate
                AND e.Situacao = 'A'
              GROUP BY DATE(e.entData)
              ORDER BY Dia DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Retorna detalhe de movimentações de um dia (vendas + compras).</summary>
        public DataTable GetMovimentacoesDia(DateTime dia)
            => GetMovimentacoesPeriodo(dia, dia);

        /// <summary>Retorna movimentações individuais (pedidos + compras) em um período.</summary>
        public DataTable GetMovimentacoesPeriodo(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"
                SELECT
                    p.pediData_Lancamento   AS Horario,
                    'Venda'                 AS Tipo,
                    p.pediNumero            AS Referencia,
                    p.pediNome_Cliente      AS Descricao,
                    COALESCE(p.pediValor_Pago, p.pediValor_Total) AS Valor,
                    p.pediValor_Total       AS ValorOriginal,
                    p.pediValor_Pago        AS ValorRecebido,
                    CASE WHEN p.pediValor_Pago IS NOT NULL
                              AND p.pediValor_Pago < p.pediValor_Total
                         THEN p.pediValor_Total - p.pediValor_Pago
                         ELSE NULL END      AS Desconto,
                    p.pediAutorizador       AS Autorizador,
                    CASE p.pediForma_Pagamento
                        WHEN 0 THEN 'Dinheiro' WHEN 1 THEN 'Cartão' WHEN 2 THEN 'Pix'
                        ELSE 'Outro' END     AS Pagamento,
                    CASE p.pediSituacao
                        WHEN 0 THEN 'Pendente'         WHEN 1 THEN 'Confirmado'
                        WHEN 2 THEN 'Em Preparo'       WHEN 3 THEN 'Pronto'
                        WHEN 4 THEN 'Saiu p/ Entrega'  WHEN 5 THEN 'Entregue'
                        WHEN 6 THEN 'Cancelado'
                        ELSE CAST(p.pediSituacao AS CHAR) END AS Status,
                    p.Codigo                AS CodigoPedido
                FROM pedido_web p
                WHERE DATE(p.pediData_Lancamento) BETWEEN @de AND @ate
                  AND p.pediSituacao NOT IN (6)
                UNION ALL
                SELECT
                    e.entData_Lancamento    AS Horario,
                    'Compra'                AS Tipo,
                    CONCAT('#', e.Codigo)  AS Referencia,
                    CONCAT('Entrada - ', e.entNome_Fornecedor) AS Descricao,
                    -e.entValorTotal        AS Valor,
                    NULL                    AS ValorOriginal,
                    NULL                    AS ValorRecebido,
                    NULL                    AS Desconto,
                    NULL                    AS Autorizador,
                    ''                      AS Pagamento,
                    'Lançado'               AS Status,
                    0                       AS CodigoPedido
                FROM entrada_mercadoria e
                WHERE DATE(e.entData) BETWEEN @de AND @ate
                  AND e.Situacao = 'A'
                ORDER BY Horario ASC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        [System.Obsolete("Use GetMovimentacoesPeriodo instead")]
        public DataTable GetMovimentacoesDia_Old(DateTime dia)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"
                SELECT
                    p.pediData_Lancamento   AS Horario,
                    'Venda'                 AS Tipo,
                    p.pediNumero            AS Referencia,
                    p.pediNome_Cliente      AS Descricao,
                    COALESCE(p.pediValor_Pago, p.pediValor_Total) AS Valor,
                    p.pediValor_Total       AS ValorOriginal,
                    p.pediValor_Pago        AS ValorRecebido,
                    CASE WHEN p.pediValor_Pago IS NOT NULL
                              AND p.pediValor_Pago < p.pediValor_Total
                         THEN p.pediValor_Total - p.pediValor_Pago
                         ELSE NULL END      AS Desconto,
                    p.pediAutorizador       AS Autorizador,
                    CASE p.pediForma_Pagamento
                        WHEN 0 THEN 'Dinheiro' WHEN 1 THEN 'Cartão' WHEN 2 THEN 'Pix'
                        ELSE 'Outro' END     AS Pagamento,
                    CASE p.pediSituacao
                        WHEN 0 THEN 'Pendente'         WHEN 1 THEN 'Confirmado'
                        WHEN 2 THEN 'Em Preparo'       WHEN 3 THEN 'Pronto'
                        WHEN 4 THEN 'Saiu p/ Entrega'  WHEN 5 THEN 'Entregue'
                        WHEN 6 THEN 'Cancelado'
                        ELSE CAST(p.pediSituacao AS CHAR) END AS Status,
                    p.Codigo                AS CodigoPedido
                FROM pedido_web p
                WHERE DATE(p.pediData_Lancamento) = @dia
                  AND p.pediSituacao NOT IN (6)
                UNION ALL
                SELECT
                    e.entData_Lancamento    AS Horario,
                    'Compra'                AS Tipo,
                    CONCAT('#', e.Codigo)  AS Referencia,
                    CONCAT('Entrada - ', e.entNome_Fornecedor) AS Descricao,
                    -e.entValorTotal        AS Valor,
                    NULL                    AS ValorOriginal,
                    NULL                    AS ValorRecebido,
                    NULL                    AS Desconto,
                    NULL                    AS Autorizador,
                    ''                      AS Pagamento,
                    'Lançado'               AS Status,
                    0                       AS CodigoPedido
                FROM entrada_mercadoria e
                WHERE DATE(e.entData) = @dia
                  AND e.Situacao = 'A'
                ORDER BY Horario ASC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dia", dia.Date);
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
                pediSubtotal          = r["pediSubtotal"] == DBNull.Value ? 0m : Convert.ToDecimal(r["pediSubtotal"]),
                pediTaxa_Entrega      = r["pediTaxa_Entrega"] == DBNull.Value ? 0m : Convert.ToDecimal(r["pediTaxa_Entrega"]),
                pediDesconto          = r["pediDesconto"] == DBNull.Value ? 0m : Convert.ToDecimal(r["pediDesconto"]),
                pediValor_Total       = Convert.ToDecimal(r["pediValor_Total"]),
                pediEndereco_Entrega  = r["pediEndereco_Entrega"]?.ToString() ?? "",
                pediObservacoes       = r["pediObservacoes"]?.ToString() ?? "",
                pediOrigem            = r["pediOrigem"] == DBNull.Value ? 0 : Convert.ToInt32(r["pediOrigem"]),
                pediValor_Pago        = r["pediValor_Pago"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(r["pediValor_Pago"]),
                pediPago_Dinheiro     = r["pediPago_Dinheiro"] == DBNull.Value ? 0m : Convert.ToDecimal(r["pediPago_Dinheiro"]),
                pediPago_Cartao       = r["pediPago_Cartao"]   == DBNull.Value ? 0m : Convert.ToDecimal(r["pediPago_Cartao"]),
                pediPago_Pix          = r["pediPago_Pix"]      == DBNull.Value ? 0m : Convert.ToDecimal(r["pediPago_Pix"]),
                pediData_Lancamento   = Convert.ToDateTime(r["pediData_Lancamento"]),
                pediCancelado_Por     = r["pediCancelado_Por"] == DBNull.Value ? null : r["pediCancelado_Por"]?.ToString(),
                pediAutorizador       = r["pediAutorizador"] == DBNull.Value ? null : r["pediAutorizador"]?.ToString(),
                pediCodigo_Cupom      = r["pediCodigo_Cupom"] == DBNull.Value ? "" : r["pediCodigo_Cupom"]?.ToString() ?? "",
                Info                  = r["Info"]?.ToString() ?? "",
            };
        }

        /// <summary>Busca um pedido completo pelo número visível (pediNumero).</summary>
        public PedidoWeb PesquisaPorNumero(string numero)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM pedido_web WHERE pediNumero = @num ORDER BY Codigo DESC LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@num", numero);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapearPedido(r);
        }
    }
}

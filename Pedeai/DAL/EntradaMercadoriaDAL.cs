using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class EntradaMercadoriaDAL : BaseDAL
    {
        // ── Listar entradas ──────────────────────────────────────────────────
        public DataTable Listar(DateTime? de = null, DateTime? ate = null, int? codigoFornecedor = null)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"
                SELECT  e.Codigo,
                        e.entData,
                        e.entNome_Fornecedor    AS Fornecedor,
                        e.entNumeroDoc          AS Documento,
                        (SELECT COUNT(*) FROM item_entrada_mercadoria i WHERE i.Codigo_Entrada = e.Codigo) AS Itens,
                        e.entValorTotal         AS Total,
                        e.entObservacoes        AS Observacoes,
                        e.entData_Lancamento    AS Lancamento
                FROM entrada_mercadoria e
                WHERE e.Situacao = 'A'";
            if (de.HasValue)  sql += " AND e.entData  >= @de";
            if (ate.HasValue) sql += " AND e.entData  <= @ate";
            if (codigoFornecedor.HasValue) sql += " AND e.Codigo_Fornecedor = @forn";
            sql += " ORDER BY e.entData DESC, e.Codigo DESC LIMIT 300";

            using var cmd = new MySqlCommand(sql, conn);
            if (de.HasValue)  cmd.Parameters.AddWithValue("@de",  de.Value.Date);
            if (ate.HasValue) cmd.Parameters.AddWithValue("@ate", ate.Value.Date);
            if (codigoFornecedor.HasValue) cmd.Parameters.AddWithValue("@forn", codigoFornecedor.Value);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public EntradaMercadoria PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "SELECT * FROM entrada_mercadoria WHERE Codigo=@cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Mapear(r) : null;
        }

        public List<ItemEntradaMercadoria> ListarItens(int codigoEntrada)
        {
            var lista = new List<ItemEntradaMercadoria>();
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "SELECT * FROM item_entrada_mercadoria WHERE Codigo_Entrada=@cod ORDER BY Codigo", conn);
            cmd.Parameters.AddWithValue("@cod", codigoEntrada);
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(MapearItem(r));
            return lista;
        }

        // ── Inserir (com baixa de estoque em transacao) ──────────────────────
        public string Inserir(EntradaMercadoria entrada, List<ItemEntradaMercadoria> itens,
            List<Modelo.ParcelaEntradaMercadoria> parcelas = null)
        {
            try
            {
                using var conn  = AbrirConexao();
                using var trans = conn.BeginTransaction();

                entrada.Codigo    = ProximoCodigo("entrada_mercadoria",      conn, trans);
                entrada.auxCodigo = ProximoAuxCodigo("entrada_mercadoria",   conn, trans);
                entrada.entData_Lancamento = DateTime.Now;

                const string sqlE = @"
                    INSERT INTO entrada_mercadoria
                        (auxCodigo, Codigo, Codigo_Fornecedor, entNome_Fornecedor,
                         entData, entNumeroDoc, entObservacoes, entValorTotal,
                         Situacao, Info, entData_Lancamento)
                    VALUES
                        (@aux,@cod,@forn,@nomeForn,
                         @dt,@doc,@obs,@total,
                         'A','',@lancamento)";
                using var cmdE = new MySqlCommand(sqlE, conn, trans);
                cmdE.Parameters.AddWithValue("@aux",       entrada.auxCodigo);
                cmdE.Parameters.AddWithValue("@cod",       entrada.Codigo);
                cmdE.Parameters.AddWithValue("@forn",      entrada.Codigo_Fornecedor > 0 ? (object)entrada.Codigo_Fornecedor : DBNull.Value);
                cmdE.Parameters.AddWithValue("@nomeForn",  entrada.entNome_Fornecedor ?? "");
                cmdE.Parameters.AddWithValue("@dt",        entrada.entData.Date);
                cmdE.Parameters.AddWithValue("@doc",       entrada.entNumeroDoc ?? "");
                cmdE.Parameters.AddWithValue("@obs",       entrada.entObservacoes ?? "");
                cmdE.Parameters.AddWithValue("@total",     entrada.entValorTotal);
                cmdE.Parameters.AddWithValue("@lancamento",entrada.entData_Lancamento);
                cmdE.ExecuteNonQuery();

                foreach (var item in itens)
                {
                    item.Codigo        = ProximoCodigo("item_entrada_mercadoria",    conn, trans);
                    item.auxCodigo     = ProximoAuxCodigo("item_entrada_mercadoria", conn, trans);
                    item.Codigo_Entrada = entrada.Codigo;

                    const string sqlI = @"
                        INSERT INTO item_entrada_mercadoria
                            (auxCodigo,Codigo,Codigo_Entrada,Codigo_Mercadoria,
                             itmNome_Mercadoria,itmQtde,itmPreco_Custo,itmSubtotal,
                             itmAtualizar_Custo,Situacao)
                        VALUES
                            (@aux,@cod,@ent,@merc,
                             @nome,@qtde,@custo,@sub,
                             @atualizar,'A')";
                    using var cmdI = new MySqlCommand(sqlI, conn, trans);
                    cmdI.Parameters.AddWithValue("@aux",      item.auxCodigo);
                    cmdI.Parameters.AddWithValue("@cod",      item.Codigo);
                    cmdI.Parameters.AddWithValue("@ent",      item.Codigo_Entrada);
                    cmdI.Parameters.AddWithValue("@merc",     item.Codigo_Mercadoria);
                    cmdI.Parameters.AddWithValue("@nome",     item.itmNome_Mercadoria ?? "");
                    cmdI.Parameters.AddWithValue("@qtde",     item.itmQtde);
                    cmdI.Parameters.AddWithValue("@custo",    item.itmPreco_Custo);
                    cmdI.Parameters.AddWithValue("@sub",      item.itmSubtotal);
                    cmdI.Parameters.AddWithValue("@atualizar",item.itmAtualizar_Custo ? 1 : 0);
                    cmdI.ExecuteNonQuery();

                    // Entrada no estoque
                    if (item.Codigo_Mercadoria > 0)
                    {
                        using var cmdEst = new MySqlCommand(
                            "UPDATE mercadoria SET mercEstoque_Atual = mercEstoque_Atual + @qtde WHERE Codigo=@merc",
                            conn, trans);
                        cmdEst.Parameters.AddWithValue("@qtde", item.itmQtde);
                        cmdEst.Parameters.AddWithValue("@merc", item.Codigo_Mercadoria);
                        cmdEst.ExecuteNonQuery();

                        // Atualiza custo se solicitado
                        if (item.itmAtualizar_Custo && item.itmPreco_Custo > 0)
                        {
                            using var cmdCusto = new MySqlCommand(
                                "UPDATE mercadoria SET mercPreco_Custo=@custo WHERE Codigo=@merc",
                                conn, trans);
                            cmdCusto.Parameters.AddWithValue("@custo", item.itmPreco_Custo);
                            cmdCusto.Parameters.AddWithValue("@merc",  item.Codigo_Mercadoria);
                            cmdCusto.ExecuteNonQuery();
                        }
                    }
                }

                // Inserir parcelas (se houver)
                if (parcelas != null && parcelas.Count > 0)
                {
                    foreach (var p in parcelas)
                    {
                        p.Codigo_Entrada = entrada.Codigo;
                        p.Codigo    = ProximoCodigo("parcela_entrada_mercadoria",    conn, trans);
                        p.auxCodigo = ProximoAuxCodigo("parcela_entrada_mercadoria", conn, trans);
                        const string sqlP = @"INSERT INTO parcela_entrada_mercadoria
                            (auxCodigo,Codigo,Codigo_Entrada,parNumero,parVencimento,parValor,parObservacao,Situacao)
                            VALUES (@aux,@cod,@ent,@num,@vcto,@val,@obs,'A')";
                        using var cmdP = new MySqlCommand(sqlP, conn, trans);
                        cmdP.Parameters.AddWithValue("@aux", p.auxCodigo);
                        cmdP.Parameters.AddWithValue("@cod", p.Codigo);
                        cmdP.Parameters.AddWithValue("@ent", p.Codigo_Entrada);
                        cmdP.Parameters.AddWithValue("@num", p.parNumero);
                        cmdP.Parameters.AddWithValue("@vcto",p.parVencimento.Date);
                        cmdP.Parameters.AddWithValue("@val", p.parValor);
                        cmdP.Parameters.AddWithValue("@obs", p.parObservacao ?? "");
                        cmdP.ExecuteNonQuery();
                    }
                }

                trans.Commit();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        // ── Cancelar entrada (reverte estoque) ───────────────────────────────
        public string Cancelar(int codigo)
        {
            try
            {
                using var conn  = AbrirConexao();
                using var trans = conn.BeginTransaction();

                // Reverte estoque dos itens
                using var cmdRev = new MySqlCommand(@"
                    UPDATE mercadoria m
                    JOIN item_entrada_mercadoria i ON i.Codigo_Mercadoria = m.Codigo
                    SET m.mercEstoque_Atual = m.mercEstoque_Atual - i.itmQtde
                    WHERE i.Codigo_Entrada = @cod", conn, trans);
                cmdRev.Parameters.AddWithValue("@cod", codigo);
                cmdRev.ExecuteNonQuery();

                using var cmdSit = new MySqlCommand(
                    "UPDATE entrada_mercadoria SET Situacao='C' WHERE Codigo=@cod", conn, trans);
                cmdSit.Parameters.AddWithValue("@cod", codigo);
                cmdSit.ExecuteNonQuery();

                trans.Commit();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public decimal TotalPeriodo(DateTime de, DateTime ate)
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                @"SELECT COALESCE(SUM(entValorTotal), 0)
                  FROM entrada_mercadoria
                  WHERE Situacao = 'A'
                    AND entData >= @de
                    AND entData <= @ate", conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            return Convert.ToDecimal(cmd.ExecuteScalar());
        }

        // ── Mapear ───────────────────────────────────────────────────────────
        private static EntradaMercadoria Mapear(MySqlDataReader r) => new EntradaMercadoria
        {
            Codigo             = Convert.ToInt32(r["Codigo"]),
            auxCodigo          = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
            Codigo_Fornecedor  = r["Codigo_Fornecedor"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Fornecedor"]),
            entNome_Fornecedor = r["entNome_Fornecedor"]?.ToString() ?? "",
            entData            = Convert.ToDateTime(r["entData"]),
            entNumeroDoc       = r["entNumeroDoc"]?.ToString()       ?? "",
            entObservacoes     = r["entObservacoes"]?.ToString()     ?? "",
            entValorTotal      = r["entValorTotal"] == DBNull.Value ? 0m : Convert.ToDecimal(r["entValorTotal"]),
            Situacao           = r["Situacao"]?.ToString()           ?? "A",
            Info               = r["Info"]?.ToString()              ?? "",
            entData_Lancamento = r["entData_Lancamento"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["entData_Lancamento"]),
        };

        private static ItemEntradaMercadoria MapearItem(MySqlDataReader r) => new ItemEntradaMercadoria
        {
            Codigo             = Convert.ToInt32(r["Codigo"]),
            auxCodigo          = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
            Codigo_Entrada     = Convert.ToInt32(r["Codigo_Entrada"]),
            Codigo_Mercadoria  = r["Codigo_Mercadoria"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Mercadoria"]),
            itmNome_Mercadoria = r["itmNome_Mercadoria"]?.ToString() ?? "",
            itmQtde            = r["itmQtde"]  == DBNull.Value ? 0m : Convert.ToDecimal(r["itmQtde"]),
            itmPreco_Custo     = r["itmPreco_Custo"] == DBNull.Value ? 0m : Convert.ToDecimal(r["itmPreco_Custo"]),
            itmSubtotal        = r["itmSubtotal"] == DBNull.Value ? 0m : Convert.ToDecimal(r["itmSubtotal"]),
            itmAtualizar_Custo = r["itmAtualizar_Custo"] == DBNull.Value ? true : Convert.ToInt32(r["itmAtualizar_Custo"]) == 1,
            Situacao           = r["Situacao"]?.ToString() ?? "A",
        };
    }
}

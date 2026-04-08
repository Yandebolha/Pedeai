using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class ParcelaEntradaMercadoriaDAL : BaseDAL
    {
        public List<ParcelaEntradaMercadoria> ListarPorEntrada(int codigoEntrada)
        {
            var lista = new List<ParcelaEntradaMercadoria>();
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "SELECT * FROM parcela_entrada_mercadoria WHERE Codigo_Entrada=@cod ORDER BY parNumero", conn);
            cmd.Parameters.AddWithValue("@cod", codigoEntrada);
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(Mapear(r));
            return lista;
        }

        /// <summary>Lista parcelas em aberto / vencimento próximo para avisos.</summary>
        public DataTable ListarAvisos(int diasAdiantados = 30)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"
                SELECT p.Codigo,
                       p.Codigo_Entrada        AS CodEntrada,
                       e.entNome_Fornecedor    AS Fornecedor,
                       p.parNumero             AS Parcela,
                       (SELECT COUNT(*) FROM parcela_entrada_mercadoria x WHERE x.Codigo_Entrada = p.Codigo_Entrada) AS TotalParcelas,
                       p.parVencimento         AS Vencimento,
                       p.parValor              AS Valor,
                       p.parObservacao         AS Observacao,
                       p.Situacao
                FROM parcela_entrada_mercadoria p
                JOIN entrada_mercadoria e ON e.Codigo = p.Codigo_Entrada
                WHERE p.Situacao = 'A'
                  AND p.parVencimento <= DATE_ADD(CURDATE(), INTERVAL @dias DAY)
                ORDER BY p.parVencimento ASC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@dias", diasAdiantados);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Lista todas parcelas (para tela de avisos completa).</summary>
        public DataTable ListarTodas(string situacao = null)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var where = string.IsNullOrEmpty(situacao) ? "" : " AND p.Situacao = @sit";
            var sql = $@"
                SELECT p.Codigo,
                       p.Codigo_Entrada        AS CodEntrada,
                       e.entNome_Fornecedor    AS Fornecedor,
                       e.entNumeroDoc          AS Documento,
                       e.entData               AS DataEntrada,
                       p.parNumero             AS Parcela,
                       (SELECT COUNT(*) FROM parcela_entrada_mercadoria x WHERE x.Codigo_Entrada = p.Codigo_Entrada) AS TotalParcelas,
                       p.parVencimento         AS Vencimento,
                       p.parValor              AS Valor,
                       p.parObservacao         AS Observacao,
                       p.Situacao,
                       p.parData_Pagamento     AS DataPagamento
                FROM parcela_entrada_mercadoria p
                JOIN entrada_mercadoria e ON e.Codigo = p.Codigo_Entrada
                WHERE 1=1{where}
                ORDER BY p.parVencimento ASC, p.Codigo_Entrada, p.parNumero";
            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrEmpty(situacao)) cmd.Parameters.AddWithValue("@sit", situacao);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public string InserirLote(List<ParcelaEntradaMercadoria> parcelas, MySqlConnection conn, MySqlTransaction trans)
        {
            try
            {
                foreach (var p in parcelas)
                {
                    p.Codigo    = ProximoCodigo("parcela_entrada_mercadoria",    conn, trans);
                    p.auxCodigo = ProximoAuxCodigo("parcela_entrada_mercadoria", conn, trans);
                    const string sql = @"INSERT INTO parcela_entrada_mercadoria
                        (auxCodigo, Codigo, Codigo_Entrada, parNumero, parVencimento, parValor, parObservacao, Situacao)
                        VALUES (@aux,@cod,@ent,@num,@vcto,@val,@obs,'A')";
                    using var cmd = new MySqlCommand(sql, conn, trans);
                    cmd.Parameters.AddWithValue("@aux", p.auxCodigo);
                    cmd.Parameters.AddWithValue("@cod", p.Codigo);
                    cmd.Parameters.AddWithValue("@ent", p.Codigo_Entrada);
                    cmd.Parameters.AddWithValue("@num", p.parNumero);
                    cmd.Parameters.AddWithValue("@vcto",p.parVencimento.Date);
                    cmd.Parameters.AddWithValue("@val", p.parValor);
                    cmd.Parameters.AddWithValue("@obs", p.parObservacao ?? "");
                    cmd.ExecuteNonQuery();
                }
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Lista parcelas filtradas por estado de vencimento.</summary>
        public DataTable ListarPorVencimento(string tipo)
        {
            var dt   = new DataTable();
            using var conn = AbrirConexao();
            string where;
            switch (tipo)
            {
                case "vencida": where = "p.Situacao='A' AND p.parVencimento < CURDATE()"; break;
                case "hoje":    where = "p.Situacao='A' AND p.parVencimento = CURDATE()"; break;
                case "7dias":   where = "p.Situacao='A' AND p.parVencimento BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 7 DAY)"; break;
                case "30dias":  where = "p.Situacao='A' AND p.parVencimento BETWEEN CURDATE() AND DATE_ADD(CURDATE(), INTERVAL 30 DAY)"; break;
                default:        where = "p.Situacao='A'"; break;
            }
            var sql = $@"
                SELECT p.Codigo,
                       p.Codigo_Entrada        AS CodEntrada,
                       e.entNome_Fornecedor    AS Fornecedor,
                       e.entNumeroDoc          AS Documento,
                       e.entData               AS DataEntrada,
                       p.parNumero             AS Parcela,
                       (SELECT COUNT(*) FROM parcela_entrada_mercadoria x WHERE x.Codigo_Entrada = p.Codigo_Entrada) AS TotalParcelas,
                       p.parVencimento         AS Vencimento,
                       p.parValor              AS Valor,
                       p.parObservacao         AS Observacao,
                       p.Situacao,
                       p.parData_Pagamento     AS DataPagamento
                FROM parcela_entrada_mercadoria p
                JOIN entrada_mercadoria e ON e.Codigo = p.Codigo_Entrada
                WHERE {where}
                ORDER BY p.parVencimento ASC, p.Codigo_Entrada, p.parNumero";
            using var cmd = new MySqlCommand(sql, conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public string MarcarPago(int codigo)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd  = new MySqlCommand(
                    @"UPDATE parcela_entrada_mercadoria
                      SET Situacao           = 'P',
                          parData_Pagamento  = CURDATE(),
                          parObservacao      = CASE
                              WHEN parObservacao IS NULL OR TRIM(parObservacao) = ''
                                  THEN CONCAT('Pago em ', DATE_FORMAT(CURDATE(),'%d/%m/%Y'))
                              ELSE CONCAT(parObservacao, ' | Pago em ', DATE_FORMAT(CURDATE(),'%d/%m/%Y'))
                          END
                      WHERE Codigo = @cod", conn);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public int ContarVencendo(int diasAdiantados = 7)
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "SELECT COUNT(*) FROM parcela_entrada_mercadoria WHERE Situacao='A' AND parVencimento <= DATE_ADD(CURDATE(), INTERVAL @d DAY)", conn);
            cmd.Parameters.AddWithValue("@d", diasAdiantados);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        private static ParcelaEntradaMercadoria Mapear(MySqlDataReader r) => new ParcelaEntradaMercadoria
        {
            Codigo             = Convert.ToInt32(r["Codigo"]),
            auxCodigo          = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
            Codigo_Entrada     = Convert.ToInt32(r["Codigo_Entrada"]),
            parNumero          = Convert.ToInt32(r["parNumero"]),
            parVencimento      = Convert.ToDateTime(r["parVencimento"]),
            parValor           = r["parValor"] == DBNull.Value ? 0m : Convert.ToDecimal(r["parValor"]),
            parObservacao      = r["parObservacao"]?.ToString() ?? "",
            Situacao           = r["Situacao"]?.ToString() ?? "A",
            parData_Pagamento  = r["parData_Pagamento"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(r["parData_Pagamento"]),
        };
    }
}

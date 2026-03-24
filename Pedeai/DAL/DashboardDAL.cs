using System;
using System.Data;
using MySqlConnector;

namespace Pedeai.DAL
{
    public class DashboardDAL : BaseDAL
    {
        public (int pedidosHoje, decimal faturamentoHoje, int clientesTotal, int pedidosPendentes)
            GetEstatisticas()
        {
            using var conn = AbrirConexao();
            var sql = @"SELECT
                (SELECT COUNT(*) FROM pedido_web
                 WHERE DATE(pediData_Lancamento) = CURDATE() AND pediSituacao <> 6) AS pedidosHoje,
                (SELECT COALESCE(SUM(pediValor_Total),0) FROM pedido_web
                 WHERE DATE(pediData_Lancamento) = CURDATE() AND pediSituacao <> 6) AS faturamento,
                (SELECT COUNT(*) FROM cliente) AS clientes,
                (SELECT COUNT(*) FROM pedido_web WHERE pediSituacao IN (0,1,2)) AS pendentes";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            if (r.Read())
                return (r.GetInt32(0), r.GetDecimal(1), r.GetInt32(2), r.GetInt32(3));
            return (0, 0m, 0, 0);
        }

        public DataRow GetLoja()
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand("SELECT * FROM loja LIMIT 1", conn);
            var dt = new DataTable();
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt.Rows.Count > 0 ? dt.Rows[0] : null;
        }
    }
}

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

        public DataTable GetVendasPorCanal(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT
                CASE pediOrigem
                    WHEN 0 THEN 'Web'
                    WHEN 1 THEN 'App'
                    WHEN 2 THEN 'Manual'
                    WHEN 3 THEN 'Manual'
                    ELSE 'Outro'
                END AS Canal,
                COUNT(*) AS Pedidos,
                COALESCE(SUM(pediValor_Total), 0) AS TotalVendas
                FROM pedido_web
                WHERE DATE(pediData_Lancamento) BETWEEN @de AND @ate
                  AND pediSituacao <> 6
                GROUP BY pediOrigem
                ORDER BY TotalVendas DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public DataTable GetTopProdutos(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT i.itpwNome_Mercadoria AS Produto,
                CAST(SUM(i.itpwQtde) AS UNSIGNED) AS Quantidade,
                COALESCE(SUM(i.itpwSubtotal), 0) AS TotalVendas
                FROM itens_pedido_web i
                JOIN pedido_web p ON p.Codigo = i.Codigo_Pedido
                WHERE DATE(p.pediData_Lancamento) BETWEEN @de AND @ate
                  AND p.pediSituacao <> 6
                GROUP BY i.itpwNome_Mercadoria
                ORDER BY Quantidade DESC
                LIMIT 8";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public DataTable GetVendasPorDia(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT DATE(pediData_Lancamento) AS Dia,
                COUNT(*) AS Pedidos,
                COALESCE(SUM(pediValor_Total), 0) AS TotalVendas
                FROM pedido_web
                WHERE DATE(pediData_Lancamento) BETWEEN @de AND @ate
                  AND pediSituacao <> 6
                GROUP BY DATE(pediData_Lancamento)
                ORDER BY Dia ASC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }
    }
}

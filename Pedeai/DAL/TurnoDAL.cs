using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class TurnoDAL : BaseDAL
    {
        public void Abrir(Turno t)
        {
            using var conn = AbrirConexao();
            t.Codigo = ProximoCodigo("turno", conn);
            var sql = @"INSERT INTO turno
                        (auxCodigo, Codigo, turAbertura, turFechamento,
                         turUsuario, turCaixa_Inicial, turCaixa_Final,
                         turObservacao, turSituacao)
                        VALUES
                        (@aux, @cod, @aber, NULL,
                         @usu, @ini, NULL,
                         @obs, 'A')";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@aux",  t.auxCodigo);
            cmd.Parameters.AddWithValue("@cod",  t.Codigo);
            cmd.Parameters.AddWithValue("@aber", t.turAbertura);
            cmd.Parameters.AddWithValue("@usu",  t.turUsuario  ?? "");
            cmd.Parameters.AddWithValue("@ini",  t.turCaixa_Inicial);
            cmd.Parameters.AddWithValue("@obs",  t.turObservacao ?? "");
            cmd.ExecuteNonQuery();
        }

        public void Fechar(int codigo, decimal caixaFinal, string observacao)
        {
            using var conn = AbrirConexao();
            var sql = @"UPDATE turno
                        SET turFechamento  = NOW(),
                            turCaixa_Final = @fin,
                            turObservacao  = @obs,
                            turSituacao    = 'F'
                        WHERE Codigo = @cod";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@fin", caixaFinal);
            cmd.Parameters.AddWithValue("@obs", observacao ?? "");
            cmd.Parameters.AddWithValue("@cod", codigo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>Retorna o turno aberto, ou null se não houver.</summary>
        public Turno GetAtivo()
        {
            using var conn = AbrirConexao();
            var sql = "SELECT * FROM turno WHERE turSituacao='A' ORDER BY Codigo DESC LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            using var rdr = cmd.ExecuteReader();
            if (!rdr.Read()) return null;
            return Mapear(rdr);
        }

        public DataTable Listar(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT Codigo, turAbertura, turFechamento, turUsuario,
                               turCaixa_Inicial, turCaixa_Final, turObservacao, turSituacao
                        FROM turno
                        WHERE DATE(turAbertura) BETWEEN @de AND @ate
                        ORDER BY Codigo DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Retorna os pedidos finalizados durante o período de um turno.</summary>
        public DataTable GetPedidosTurno(DateTime abertura, DateTime fechamento)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT p.Codigo, p.pediNome_Cliente, p.pediSituacao,
                               p.pediForma_Pagamento, p.pediTipo_Entrega,
                               p.pediSubtotal, p.pediTaxa_Entrega, p.pediDesconto, p.pediValor_Total,
                               p.pediPago_Dinheiro, p.pediPago_Cartao, COALESCE(p.pediPago_CartaoDebito,0) AS pediPago_CartaoDebito, p.pediPago_Pix,
                               p.pediValor_Pago, p.pediData_Lancamento, p.pediData_Atualizacao
                        FROM pedido_web p
                        WHERE p.pediData_Lancamento >= @de
                          AND p.pediData_Lancamento <= @ate
                          AND p.pediSituacao != 6
                        ORDER BY p.Codigo";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  abertura);
            cmd.Parameters.AddWithValue("@ate", fechamento);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        private static Turno Mapear(MySqlDataReader r)
        {
            return new Turno
            {
                Codigo           = r.GetInt32("Codigo"),
                turAbertura      = r.GetDateTime("turAbertura"),
                turFechamento    = r.IsDBNull(r.GetOrdinal("turFechamento")) ? (DateTime?)null : r.GetDateTime("turFechamento"),
                turUsuario       = r.IsDBNull(r.GetOrdinal("turUsuario"))    ? "" : r.GetString("turUsuario"),
                turCaixa_Inicial = r.GetDecimal("turCaixa_Inicial"),
                turCaixa_Final   = r.IsDBNull(r.GetOrdinal("turCaixa_Final")) ? (decimal?)null : r.GetDecimal("turCaixa_Final"),
                turObservacao    = r.IsDBNull(r.GetOrdinal("turObservacao"))  ? "" : r.GetString("turObservacao"),
                turSituacao      = r.GetChar("turSituacao")
            };
        }
    }
}

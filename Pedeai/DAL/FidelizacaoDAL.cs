using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class FidelizacaoDAL : BaseDAL
    {
        public ConfigFidelizacao Carregar()
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM config_fidelizacao WHERE Codigo = 1 LIMIT 1", conn);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return new ConfigFidelizacao();
            return new ConfigFidelizacao
            {
                Codigo            = 1,
                fidAtivo          = r["fidAtivo"] != DBNull.Value && Convert.ToBoolean(r["fidAtivo"]),
                fidMeta_Gasto     = r["fidMeta_Gasto"] == DBNull.Value ? 500m : Convert.ToDecimal(r["fidMeta_Gasto"]),
                fidPremio_Tipo    = r["fidPremio_Tipo"]?.ToString() ?? "CUPOM",
                fidCupom_Tipo     = r["fidCupom_Tipo"]?.ToString() ?? "PERCENTUAL",
                fidCupom_Valor    = r["fidCupom_Valor"] == DBNull.Value ? 10m : Convert.ToDecimal(r["fidCupom_Valor"]),
                fidCupom_Minimo   = r["fidCupom_Minimo"] == DBNull.Value ? 0m : Convert.ToDecimal(r["fidCupom_Minimo"]),
                fidCupom_Validade = r["fidCupom_Validade"] == DBNull.Value ? 30 : Convert.ToInt32(r["fidCupom_Validade"]),
                fidProduto_Codigo = r["fidProduto_Codigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["fidProduto_Codigo"]),
                fidProduto_Nome   = r["fidProduto_Nome"]?.ToString() ?? "",
                fidMensagem       = r["fidMensagem"]?.ToString() ?? "",
                Info              = r["Info"]?.ToString() ?? "",
            };
        }

        public string Salvar(ConfigFidelizacao cfg)
        {
            try
            {
                using var conn = AbrirConexao();
                var sql = @"INSERT INTO config_fidelizacao
                    (Codigo, fidAtivo, fidMeta_Gasto, fidPremio_Tipo, fidCupom_Tipo, fidCupom_Valor,
                     fidCupom_Minimo, fidCupom_Validade, fidProduto_Codigo, fidProduto_Nome, fidMensagem, Info)
                    VALUES (1, @ativo, @meta, @tipo, @ctipo, @cval, @cmin, @cvalid, @pcod, @pnom, @msg, '')
                    ON DUPLICATE KEY UPDATE
                        fidAtivo=@ativo, fidMeta_Gasto=@meta, fidPremio_Tipo=@tipo,
                        fidCupom_Tipo=@ctipo, fidCupom_Valor=@cval, fidCupom_Minimo=@cmin,
                        fidCupom_Validade=@cvalid, fidProduto_Codigo=@pcod, fidProduto_Nome=@pnom,
                        fidMensagem=@msg";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@ativo",  cfg.fidAtivo ? 1 : 0);
                cmd.Parameters.AddWithValue("@meta",   cfg.fidMeta_Gasto);
                cmd.Parameters.AddWithValue("@tipo",   cfg.fidPremio_Tipo ?? "CUPOM");
                cmd.Parameters.AddWithValue("@ctipo",  cfg.fidCupom_Tipo ?? "PERCENTUAL");
                cmd.Parameters.AddWithValue("@cval",   cfg.fidCupom_Valor);
                cmd.Parameters.AddWithValue("@cmin",   cfg.fidCupom_Minimo);
                cmd.Parameters.AddWithValue("@cvalid", cfg.fidCupom_Validade);
                cmd.Parameters.AddWithValue("@pcod",   cfg.fidProduto_Codigo > 0 ? (object)cfg.fidProduto_Codigo : DBNull.Value);
                cmd.Parameters.AddWithValue("@pnom",   cfg.fidProduto_Nome ?? "");
                cmd.Parameters.AddWithValue("@msg",    cfg.fidMensagem ?? "");
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Quantas vezes esse cliente já recebeu um prêmio de fidelização.</summary>
        public int ContarPremiosEnviados(int codigoCliente)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM historico_fidelizacao WHERE Codigo_Cliente = @cli", conn);
            cmd.Parameters.AddWithValue("@cli", codigoCliente);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value ? 0 : Convert.ToInt32(r);
        }

        public void RegistrarHistorico(int codigoCliente, int codigoPedido,
                                       string cupomCodigo, string descricao, string telefone)
        {
            using var conn = AbrirConexao();
            var sql = @"INSERT INTO historico_fidelizacao
                        (Codigo_Cliente, Codigo_Pedido, fidCupomCodigo, fidDescricao, fidTelefone, fidData)
                        VALUES (@cli, @ped, @cup, @desc, @tel, NOW())";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cli",  codigoCliente);
            cmd.Parameters.AddWithValue("@ped",  codigoPedido);
            cmd.Parameters.AddWithValue("@cup",  cupomCodigo ?? "");
            cmd.Parameters.AddWithValue("@desc", descricao ?? "");
            cmd.Parameters.AddWithValue("@tel",  telefone ?? "");
            cmd.ExecuteNonQuery();
        }

        public DataTable ListarHistorico(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT h.Codigo,
                               h.fidData           AS Data,
                               COALESCE(c.clieNome_RazaoSocial, '—') AS Cliente,
                               h.fidTelefone       AS Telefone,
                               h.fidCupomCodigo    AS Cupom,
                               h.fidDescricao      AS Descricao
                        FROM historico_fidelizacao h
                        LEFT JOIN cliente c ON c.Codigo = h.Codigo_Cliente
                        WHERE DATE(h.fidData) BETWEEN @de AND @ate
                        ORDER BY h.fidData DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Insere um cupom de fidelização na tabela cupom (uso único).</summary>
        public void CriarCupomFidelizacao(string codigo, ConfigFidelizacao cfg)
        {
            using var conn = AbrirConexao();
            int nextCod  = ProximoCodigo("cupom", conn);
            int nextAux  = ProximoAuxCodigo("cupom", conn);
            var sql = @"INSERT INTO cupom
                (auxCodigo, Codigo, cupomCodigo, cupomDescricao, cupomTipo, cupomValor,
                 cupomPedido_Minimo, cupomLimite_Usos, cupomUsos_Realizados,
                 cupomValido_Ate, cupomData_Cadastro, Situacao, Status_Transmissao, Info)
                VALUES (@aux, @cod, @ccode, @desc, @tipo, @val, @min, 1, 0, @valid, NOW(), 'A', 'N', '')";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@aux",   nextAux);
            cmd.Parameters.AddWithValue("@cod",   nextCod);
            cmd.Parameters.AddWithValue("@ccode", codigo);
            cmd.Parameters.AddWithValue("@desc",  "Cupom de Fidelização");
            cmd.Parameters.AddWithValue("@tipo",  cfg.fidCupom_Tipo ?? "PERCENTUAL");
            cmd.Parameters.AddWithValue("@val",   cfg.fidCupom_Valor);
            cmd.Parameters.AddWithValue("@min",   cfg.fidCupom_Minimo);
            cmd.Parameters.AddWithValue("@valid", DateTime.Today.AddDays(cfg.fidCupom_Validade > 0 ? cfg.fidCupom_Validade : 30));
            cmd.ExecuteNonQuery();
        }
    }
}

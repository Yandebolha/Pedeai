using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class FidelizacaoDAL : BaseDAL
    {
        public ConfigFidelizacao Carregar(int codigo = 1)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM config_fidelizacao WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return new ConfigFidelizacao();
            return MapConfig(r);
        }

        private static ConfigFidelizacao MapConfig(MySqlConnector.MySqlDataReader r)
        {
            return new ConfigFidelizacao
            {
                Codigo            = Convert.ToInt32(r["Codigo"]),
                fidNome           = r["fidNome"] == DBNull.Value ? "Regra Padrão" : r["fidNome"].ToString(),
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

        public DataTable Listar()
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                @"SELECT Codigo, fidNome AS Nome, fidAtivo AS Ativo, fidMeta_Gasto AS Meta, fidPremio_Tipo AS Tipo
                  FROM config_fidelizacao ORDER BY Codigo", conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public string Incluir(ConfigFidelizacao cfg)
        {
            try
            {
                using var conn = AbrirConexao();
                var sql = @"INSERT INTO config_fidelizacao
                    (fidNome, fidAtivo, fidMeta_Gasto, fidPremio_Tipo, fidCupom_Tipo, fidCupom_Valor,
                     fidCupom_Minimo, fidCupom_Validade, fidProduto_Codigo, fidProduto_Nome, fidMensagem, Info)
                    VALUES (@nome, @ativo, @meta, @tipo, @ctipo, @cval, @cmin, @cvalid, @pcod, @pnom, @msg, '')";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, cfg);
                cmd.ExecuteNonQuery();
                cfg.Codigo = (int)cmd.LastInsertedId;
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(ConfigFidelizacao cfg)
        {
            try
            {
                using var conn = AbrirConexao();
                var sql = @"UPDATE config_fidelizacao SET
                    fidNome=@nome, fidAtivo=@ativo, fidMeta_Gasto=@meta, fidPremio_Tipo=@tipo,
                    fidCupom_Tipo=@ctipo, fidCupom_Valor=@cval, fidCupom_Minimo=@cmin,
                    fidCupom_Validade=@cvalid, fidProduto_Codigo=@pcod, fidProduto_Nome=@pnom,
                    fidMensagem=@msg
                    WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@cod", cfg.Codigo);
                BindParams(cmd, cfg);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Excluir(int codigo)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd = new MySqlCommand("DELETE FROM config_fidelizacao WHERE Codigo=@cod", conn);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private static void BindParams(MySqlConnector.MySqlCommand cmd, ConfigFidelizacao cfg)
        {
            cmd.Parameters.AddWithValue("@nome",  cfg.fidNome ?? "Regra Padrão");
            cmd.Parameters.AddWithValue("@ativo", cfg.fidAtivo ? 1 : 0);
            cmd.Parameters.AddWithValue("@meta",  cfg.fidMeta_Gasto);
            cmd.Parameters.AddWithValue("@tipo",  cfg.fidPremio_Tipo ?? "CUPOM");
            cmd.Parameters.AddWithValue("@ctipo", cfg.fidCupom_Tipo ?? "PERCENTUAL");
            cmd.Parameters.AddWithValue("@cval",  cfg.fidCupom_Valor);
            cmd.Parameters.AddWithValue("@cmin",  cfg.fidCupom_Minimo);
            cmd.Parameters.AddWithValue("@cvalid",cfg.fidCupom_Validade);
            cmd.Parameters.AddWithValue("@pcod",  cfg.fidProduto_Codigo > 0 ? (object)cfg.fidProduto_Codigo : DBNull.Value);
            cmd.Parameters.AddWithValue("@pnom",  cfg.fidProduto_Nome ?? "");
            cmd.Parameters.AddWithValue("@msg",   cfg.fidMensagem ?? "");
        }

        [Obsolete("Use Incluir or Alterar instead")]
        public string Salvar(ConfigFidelizacao cfg) => cfg.Codigo == 0 ? Incluir(cfg) : Alterar(cfg);

        /// <summary>Quantas vezes esse cliente já recebeu um prêmio de uma config específica.</summary>
        public int ContarPremiosEnviados(int codigoCliente, int codigoConfig)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT COUNT(*) FROM historico_fidelizacao WHERE Codigo_Cliente=@cli AND Codigo_Config=@cfg", conn);
            cmd.Parameters.AddWithValue("@cli", codigoCliente);
            cmd.Parameters.AddWithValue("@cfg", codigoConfig);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value ? 0 : Convert.ToInt32(r);
        }

        public void RegistrarHistorico(int codigoCliente, int codigoPedido, int codigoConfig,
                                       string cupomCodigo, string descricao, string telefone)
        {
            using var conn = AbrirConexao();
            var sql = @"INSERT INTO historico_fidelizacao
                        (Codigo_Cliente, Codigo_Pedido, Codigo_Config, fidCupomCodigo, fidDescricao, fidTelefone, fidData)
                        VALUES (@cli, @ped, @cfg, @cup, @desc, @tel, NOW())";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cli",  codigoCliente);
            cmd.Parameters.AddWithValue("@ped",  codigoPedido);
            cmd.Parameters.AddWithValue("@cfg",  codigoConfig);
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

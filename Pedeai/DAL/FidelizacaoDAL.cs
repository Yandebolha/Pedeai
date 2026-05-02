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
                fidCupom_Validade    = r["fidCupom_Validade"] == DBNull.Value ? 30 : Convert.ToInt32(r["fidCupom_Validade"]),
                fidCupom_Limite_Usos = r["fidCupom_Limite_Usos"] == DBNull.Value ? 1 : Convert.ToInt32(r["fidCupom_Limite_Usos"]),
                fidProduto_Codigo = r["fidProduto_Codigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["fidProduto_Codigo"]),
                fidProduto_Nome   = r["fidProduto_Nome"]?.ToString() ?? "",
                fidProduto_Qtde   = r["fidProduto_Qtde"] == DBNull.Value ? 1 : Convert.ToInt32(r["fidProduto_Qtde"]),
                fidMeta_Tipo      = r["fidMeta_Tipo"]?.ToString() ?? "VALOR",
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
                    (fidNome, fidAtivo, fidMeta_Gasto, fidMeta_Tipo, fidPremio_Tipo, fidCupom_Tipo, fidCupom_Valor,
                     fidCupom_Minimo, fidCupom_Validade, fidCupom_Limite_Usos, fidProduto_Codigo, fidProduto_Nome, fidProduto_Qtde, fidMensagem, Info)
                    VALUES (@nome, @ativo, @meta, @mtp, @tipo, @ctipo, @cval, @cmin, @cvalid, @clim, @pcod, @pnom, @pqtd, @msg, '')";
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
                    fidNome=@nome, fidAtivo=@ativo, fidMeta_Gasto=@meta, fidMeta_Tipo=@mtp, fidPremio_Tipo=@tipo,
                    fidCupom_Tipo=@ctipo, fidCupom_Valor=@cval, fidCupom_Minimo=@cmin,
                    fidCupom_Validade=@cvalid, fidCupom_Limite_Usos=@clim, fidProduto_Codigo=@pcod, fidProduto_Nome=@pnom,
                    fidProduto_Qtde=@pqtd, fidMensagem=@msg
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
            cmd.Parameters.AddWithValue("@mtp",   cfg.fidMeta_Tipo ?? "VALOR");
            cmd.Parameters.AddWithValue("@tipo",  cfg.fidPremio_Tipo ?? "CUPOM");
            cmd.Parameters.AddWithValue("@ctipo", cfg.fidCupom_Tipo ?? "PERCENTUAL");
            cmd.Parameters.AddWithValue("@cval",  cfg.fidCupom_Valor);
            cmd.Parameters.AddWithValue("@cmin",  cfg.fidCupom_Minimo);
            cmd.Parameters.AddWithValue("@cvalid",cfg.fidCupom_Validade);
            cmd.Parameters.AddWithValue("@clim",  cfg.fidCupom_Limite_Usos > 0 ? cfg.fidCupom_Limite_Usos : 1);
            cmd.Parameters.AddWithValue("@pcod",  cfg.fidProduto_Codigo > 0 ? (object)cfg.fidProduto_Codigo : DBNull.Value);
            cmd.Parameters.AddWithValue("@pnom",  cfg.fidProduto_Nome ?? "");
            cmd.Parameters.AddWithValue("@pqtd",  cfg.fidProduto_Qtde > 0 ? cfg.fidProduto_Qtde : 1);
            cmd.Parameters.AddWithValue("@msg",   cfg.fidMensagem ?? "");
        }

        [Obsolete("Use Incluir or Alterar instead")]
        public string Salvar(ConfigFidelizacao cfg) => cfg.Codigo == 0 ? Incluir(cfg) : Alterar(cfg);

        /// <summary>Conta pedidos entregues do cliente no mês/ano informado (para critério PEDIDOS).</summary>
        public int ContarPedidosClienteMes(int codigoCliente, int ano, int mes)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                @"SELECT COUNT(*) FROM pedido_web
                  WHERE Codigo_Cliente = @cli
                    AND YEAR(pediData_Lancamento) = @ano
                    AND MONTH(pediData_Lancamento) = @mes
                    AND pediSituacao != 6", conn);
            cmd.Parameters.AddWithValue("@cli", codigoCliente);
            cmd.Parameters.AddWithValue("@ano", ano);
            cmd.Parameters.AddWithValue("@mes", mes);
            var r = cmd.ExecuteScalar();
            return r == DBNull.Value ? 0 : Convert.ToInt32(r);
        }

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

        /// <summary>Quantas vezes esse cliente já recebeu um prêmio desta config no mês/ano informado.</summary>
        public int ContarPremiosEnviadosMes(int codigoCliente, int codigoConfig, int ano, int mes)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                @"SELECT COUNT(*) FROM historico_fidelizacao
                  WHERE Codigo_Cliente=@cli AND Codigo_Config=@cfg
                    AND YEAR(fidData)=@ano AND MONTH(fidData)=@mes", conn);
            cmd.Parameters.AddWithValue("@cli", codigoCliente);
            cmd.Parameters.AddWithValue("@cfg", codigoConfig);
            cmd.Parameters.AddWithValue("@ano", ano);
            cmd.Parameters.AddWithValue("@mes", mes);
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

        /// <summary>Busca o primeiro cupom de fidelização disponível (não usado, não vencido) do cliente.</summary>
        public Modelo.Cupom BuscarCupomDisponivel(int codigoCliente)
        {
            using var conn = AbrirConexao();
            var sql = @"
                SELECT c.*
                FROM historico_fidelizacao h
                JOIN cupom c ON c.cupomCodigo = h.fidCupomCodigo
                WHERE h.Codigo_Cliente = @cli
                  AND c.Situacao = 'A'
                  AND (c.cupomTipo IS NULL OR c.cupomTipo <> 'PRODUTO')
                  AND (c.cupomLimite_Usos = 0 OR c.cupomUsos_Realizados < c.cupomLimite_Usos)
                  AND c.cupomValido_Ate >= CURDATE()
                ORDER BY c.cupomValido_Ate ASC
                LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cli", codigoCliente);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Modelo.Cupom
            {
                Codigo                = Convert.ToInt32(r["Codigo"]),
                cupomCodigo           = r["cupomCodigo"]?.ToString() ?? "",
                cupomDescricao        = r["cupomDescricao"]?.ToString() ?? "Cupom de Fidelização",
                cupomTipo             = r["cupomTipo"]?.ToString() ?? "PERCENTUAL",
                cupomValor            = r["cupomValor"] == DBNull.Value ? 0m : Convert.ToDecimal(r["cupomValor"]),
                cupomPedido_Minimo    = r["cupomPedido_Minimo"] == DBNull.Value ? 0m : Convert.ToDecimal(r["cupomPedido_Minimo"]),
                cupomLimite_Usos      = r["cupomLimite_Usos"] == DBNull.Value ? 1 : Convert.ToInt32(r["cupomLimite_Usos"]),
                cupomUsos_Realizados  = r["cupomUsos_Realizados"] == DBNull.Value ? 0 : Convert.ToInt32(r["cupomUsos_Realizados"]),
                cupomValido_Ate       = r["cupomValido_Ate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(r["cupomValido_Ate"]),
                Situacao              = r["Situacao"]?.ToString() ?? "A",
            };
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
                VALUES (@aux, @cod, @ccode, @desc, @tipo, @val, @min, @lim, 0, @valid, NOW(), 'A', 'N', '')";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@aux",   nextAux);
            cmd.Parameters.AddWithValue("@cod",   nextCod);
            cmd.Parameters.AddWithValue("@ccode", codigo);
            cmd.Parameters.AddWithValue("@desc",  "Cupom de Fidelização");
            cmd.Parameters.AddWithValue("@tipo",  cfg.fidCupom_Tipo ?? "PERCENTUAL");
            cmd.Parameters.AddWithValue("@val",   cfg.fidCupom_Valor);
            cmd.Parameters.AddWithValue("@min",   cfg.fidCupom_Minimo);
            cmd.Parameters.AddWithValue("@valid", DateTime.Today.AddDays(cfg.fidCupom_Validade > 0 ? cfg.fidCupom_Validade : 30));
            cmd.Parameters.AddWithValue("@lim",   cfg.fidCupom_Limite_Usos > 0 ? cfg.fidCupom_Limite_Usos : 1);
            cmd.ExecuteNonQuery();
        }
        /// <summary>Cria um cupom de prêmio PRODUTO no MySQL para sincronização web (prefixo FIDP).</summary>
        public void CriarCupomPremioProduto(string codigo, ConfigFidelizacao cfg, decimal preco)
        {
            using var conn = AbrirConexao();
            int nextCod = ProximoCodigo("cupom", conn);
            int nextAux = ProximoAuxCodigo("cupom", conn);
            int qtde    = cfg.fidProduto_Qtde > 0 ? cfg.fidProduto_Qtde : 1;
            var sql = @"INSERT IGNORE INTO cupom
                (auxCodigo, Codigo, cupomCodigo, cupomDescricao, cupomTipo, cupomValor,
                 cupomPedido_Minimo, cupomLimite_Usos, cupomUsos_Realizados,
                 cupomValido_Ate, cupomData_Cadastro, Situacao, Status_Transmissao, Info)
                VALUES (@aux, @cod, @ccode, @desc, 'PRODUTO', @val, 0, 1, 0, @valid, NOW(), 'A', 'N', '')";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@aux",   nextAux);
            cmd.Parameters.AddWithValue("@cod",   nextCod);
            cmd.Parameters.AddWithValue("@ccode", codigo);
            cmd.Parameters.AddWithValue("@desc",  $"Prêmio produto: {qtde}x {cfg.fidProduto_Nome}");
            cmd.Parameters.AddWithValue("@val",   preco * qtde);
            cmd.Parameters.AddWithValue("@valid", DateTime.Today.AddDays(365));
            cmd.ExecuteNonQuery();
        }

        /// <summary>Retorna o prêmio PRODUTO pendente mais recente do cliente (ainda não resgatado).</summary>
        public (int historicoCod, int codigoProduto, string nomeProduto, int qtde) BuscarPremioProdutoPendente(int codigoCliente)
        {
            using var conn = AbrirConexao();
            var sql = @"
                SELECT h.Codigo, f.fidProduto_Codigo, f.fidProduto_Nome, f.fidProduto_Qtde
                FROM historico_fidelizacao h
                JOIN config_fidelizacao f ON f.Codigo = h.Codigo_Config
                WHERE h.Codigo_Cliente = @cli
                  AND h.fidCupomCodigo LIKE 'PROD:%'
                  AND h.fidCupomCodigo <> 'PROD:OK'
                ORDER BY h.fidData DESC
                LIMIT 1";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cli", codigoCliente);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return (0, 0, "", 0);
            return (
                Convert.ToInt32(r["Codigo"]),
                r["fidProduto_Codigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["fidProduto_Codigo"]),
                r["fidProduto_Nome"]?.ToString() ?? "",
                r["fidProduto_Qtde"] == DBNull.Value ? 1 : Convert.ToInt32(r["fidProduto_Qtde"])
            );
        }

        /// <summary>Marca o prêmio PRODUTO do histórico como utilizado.</summary>
        public void MarcarPremioProdutoUsado(int codigoHistorico)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "UPDATE historico_fidelizacao SET fidCupomCodigo='PROD:OK' WHERE Codigo=@id", conn);
            cmd.Parameters.AddWithValue("@id", codigoHistorico);
            cmd.ExecuteNonQuery();
        }
    }
}

using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class MercadoriaDAL : BaseDAL
    {
        /// <summary>Lista mercadorias com join na categoria.</summary>
        public DataTable Listar()
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT m.Codigo,
                               m.mercMercadoria        AS Nome,
                               g.grmeDescricao_        AS Categoria,
                               m.mercPreco_Venda        AS Preco,
                               m.mercPreco_Promocional  AS Promocional,
                               m.mercEstoque_Atual      AS Estoque,
                               m.Situacao
                        FROM mercadoria m
                        LEFT JOIN grupo_mercadoria g ON g.Codigo = m.Codigo_Grupo
                        ORDER BY m.mercMercadoria ASC";
            using var cmd = new MySqlCommand(sql, conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public Mercadoria PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM mercadoria WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapearMercadoria(r);
        }

        /// <summary>Inclui nova mercadoria. Gera Codigo/auxCodigo automaticamente.</summary>
        public string Incluir(Mercadoria obj)
        {
            try
            {
                using var conn = AbrirConexao();

                using (var chk = new MySqlCommand(
                    "SELECT COUNT(*) FROM mercadoria WHERE mercMercadoria = @nome AND Situacao = 'A'", conn))
                {
                    chk.Parameters.AddWithValue("@nome", obj.mercMercadoria);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return $"J\u00e1 existe um produto com o nome '{obj.mercMercadoria}'.";
                }

                obj.Codigo    = ProximoCodigo("mercadoria", conn);
                obj.auxCodigo = ProximoAuxCodigo("mercadoria", conn);
                obj.mercData_Cadastro = DateTime.Now;

                var sql = @"INSERT INTO mercadoria
                            (auxCodigo, Codigo, Codigo_Grupo, mercMercadoria, mercApresentacao,
                             mercPreco_Venda, mercPreco_Custo, mercPreco_Promocional, mercEstoque_Atual,
                             mercControla_Estoque, mercImagem_Url, mercDestaque, mercOrdem,
                             mercHabilitar_Ifood, mercHabilitar_Site, mercPreco_Adicional, mercAdicional_Qtd_Max, mercFracionado, mercQtd_Sabores,
                             Situacao, Status_Transmissao, Info, mercData_Cadastro)
                            VALUES(@aux, @cod, @grp, @nome, @desc, @preco, @custo, @promo, @est,
                                   @ctrl, @img, @dest, @ordem, @ifood, @site, @precoad, @qtdmaxad, @frac, @qtdsab, @sit, @trans, @info, @dt)";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Altera mercadoria existente.</summary>
        public string Alterar(Mercadoria obj)
        {
            try
            {
                using var conn = AbrirConexao();

                using (var chk = new MySqlCommand(
                    "SELECT COUNT(*) FROM mercadoria WHERE mercMercadoria = @nome AND Situacao = 'A' AND Codigo <> @cod", conn))
                {
                    chk.Parameters.AddWithValue("@nome", obj.mercMercadoria);
                    chk.Parameters.AddWithValue("@cod",  obj.Codigo);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return $"J\u00e1 existe outro produto com o nome '{obj.mercMercadoria}'.";
                }
                var sql = @"UPDATE mercadoria SET
                            Codigo_Grupo=@grp, mercMercadoria=@nome, mercApresentacao=@desc,
                            mercPreco_Venda=@preco, mercPreco_Custo=@custo, mercPreco_Promocional=@promo, mercEstoque_Atual=@est,
                            mercControla_Estoque=@ctrl, mercImagem_Url=@img, mercDestaque=@dest,
                            mercOrdem=@ordem, mercHabilitar_Ifood=@ifood, mercHabilitar_Site=@site,
                            mercPreco_Adicional=@precoad, mercAdicional_Qtd_Max=@qtdmaxad, mercFracionado=@frac, mercQtd_Sabores=@qtdsab,
                            Situacao=@sit
                            WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Alterna Situacao entre 'A' (Ativo) e 'I' (Inativo).</summary>
        public void AlternarSituacao(int codigo)
        {
            using var conn = AbrirConexao();
            var sql = "UPDATE mercadoria SET Situacao = IF(Situacao='A','I','A') WHERE Codigo = @cod";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            cmd.ExecuteNonQuery();
        }

        /// <summary>Lista produtos com estoque controlado, opcionalmente filtrando por nome.</summary>
        public DataTable ListarEstoque(string filtro = "")
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var where = string.IsNullOrWhiteSpace(filtro)
                ? ""
                : " AND m.mercMercadoria LIKE @filtro";
            var sql = $@"SELECT m.Codigo,
                               m.mercMercadoria        AS Produto,
                               g.grmeDescricao_        AS Categoria,
                               m.mercEstoque_Atual     AS Estoque,
                               m.Situacao
                        FROM mercadoria m
                        LEFT JOIN grupo_mercadoria g ON g.Codigo = m.Codigo_Grupo
                        WHERE m.mercControla_Estoque = 1{where}
                        ORDER BY m.mercMercadoria ASC";
            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(filtro))
                cmd.Parameters.AddWithValue("@filtro", "%" + filtro + "%");
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Ajusta o estoque de um produto: delta positivo = entrada, negativo = saída.</summary>
        public string AjustarEstoque(int codigoProduto, decimal delta, string tipo, string obs)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd = new MySqlCommand(
                    "UPDATE mercadoria SET mercEstoque_Atual = mercEstoque_Atual + @delta WHERE Codigo = @cod", conn);
                cmd.Parameters.AddWithValue("@delta", delta);
                cmd.Parameters.AddWithValue("@cod",   codigoProduto);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private static void BindParams(MySqlCommand cmd, Mercadoria obj)
        {
            cmd.Parameters.AddWithValue("@aux",   obj.auxCodigo);
            cmd.Parameters.AddWithValue("@cod",   obj.Codigo);
            cmd.Parameters.AddWithValue("@grp",   obj.Codigo_Grupo);
            cmd.Parameters.AddWithValue("@nome",  obj.mercMercadoria);
            cmd.Parameters.AddWithValue("@desc",  obj.mercApresentacao ?? "");
            cmd.Parameters.AddWithValue("@preco", obj.mercPreco_Venda);
            cmd.Parameters.AddWithValue("@custo", obj.mercPreco_Custo);
            cmd.Parameters.AddWithValue("@promo", obj.mercPreco_Promocional);
            cmd.Parameters.AddWithValue("@est",   obj.mercEstoque_Atual);
            cmd.Parameters.AddWithValue("@ctrl",  obj.mercControla_Estoque ? 1 : 0);
            cmd.Parameters.AddWithValue("@img",   obj.mercImagem_Url ?? "");
            cmd.Parameters.AddWithValue("@dest",  obj.mercDestaque ? 1 : 0);
            cmd.Parameters.AddWithValue("@ordem", obj.mercOrdem);
            cmd.Parameters.AddWithValue("@ifood", obj.mercHabilitar_Ifood ? 1 : 0);
            cmd.Parameters.AddWithValue("@site",    obj.mercHabilitar_Site  ? 1 : 0);
            cmd.Parameters.AddWithValue("@precoad",  obj.mercPreco_Adicional);
            cmd.Parameters.AddWithValue("@qtdmaxad", obj.mercAdicional_Qtd_Max > 0 ? obj.mercAdicional_Qtd_Max : 1);
            cmd.Parameters.AddWithValue("@frac",     obj.mercFracionado    ? 1 : 0);
            cmd.Parameters.AddWithValue("@qtdsab",   obj.mercQtd_Sabores);
            cmd.Parameters.AddWithValue("@sit",      obj.Situacao ?? "A");
            cmd.Parameters.AddWithValue("@trans", obj.Status_Transmissao ?? "N");
            cmd.Parameters.AddWithValue("@info",  obj.Info ?? "");
            cmd.Parameters.AddWithValue("@dt",    obj.mercData_Cadastro);
        }

        private static bool ColExists(MySqlDataReader r, string col)
        {
            for (int i = 0; i < r.FieldCount; i++)
                if (string.Equals(r.GetName(i), col, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        private static Mercadoria MapearMercadoria(MySqlDataReader r)
        {
            return new Mercadoria
            {
                Codigo               = Convert.ToInt32(r["Codigo"]),
                auxCodigo            = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
                Codigo_Grupo         = r["Codigo_Grupo"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Grupo"]),
                mercMercadoria       = r["mercMercadoria"]?.ToString() ?? "",
                mercApresentacao     = r["mercApresentacao"]?.ToString() ?? "",
                mercPreco_Venda      = r["mercPreco_Venda"] == DBNull.Value ? 0 : Convert.ToDecimal(r["mercPreco_Venda"]),
                mercPreco_Custo      = r["mercPreco_Custo"] == DBNull.Value ? 0 : Convert.ToDecimal(r["mercPreco_Custo"]),
                mercPreco_Promocional= r["mercPreco_Promocional"] == DBNull.Value ? 0 : Convert.ToDecimal(r["mercPreco_Promocional"]),
                mercEstoque_Atual    = r["mercEstoque_Atual"] == DBNull.Value ? 0 : Convert.ToDecimal(r["mercEstoque_Atual"]),
                mercControla_Estoque = r["mercControla_Estoque"]?.ToString() == "1" || r["mercControla_Estoque"]?.ToString() == "True",
                mercImagem_Url       = r["mercImagem_Url"]?.ToString() ?? "",
                mercDestaque         = r["mercDestaque"]?.ToString() == "1" || r["mercDestaque"]?.ToString() == "True",
                mercOrdem            = r["mercOrdem"] == DBNull.Value ? 0 : Convert.ToInt32(r["mercOrdem"]),
                mercHabilitar_Ifood  = r["mercHabilitar_Ifood"]?.ToString() == "1" || r["mercHabilitar_Ifood"]?.ToString() == "True",
                mercHabilitar_Site   = ColExists(r, "mercHabilitar_Site") &&
                                       (r["mercHabilitar_Site"]?.ToString() == "1" || r["mercHabilitar_Site"]?.ToString() == "True"),
                mercPreco_Adicional  = ColExists(r, "mercPreco_Adicional") && r["mercPreco_Adicional"] != DBNull.Value
                                       ? Convert.ToDecimal(r["mercPreco_Adicional"]) : 0m,
                mercAdicional_Qtd_Max = ColExists(r, "mercAdicional_Qtd_Max") && r["mercAdicional_Qtd_Max"] != DBNull.Value
                                       ? Convert.ToInt32(r["mercAdicional_Qtd_Max"]) : 1,
                mercFracionado       = ColExists(r, "mercFracionado") &&
                                       (r["mercFracionado"]?.ToString() == "1" || r["mercFracionado"]?.ToString() == "True"),
                mercQtd_Sabores      = ColExists(r, "mercQtd_Sabores") && r["mercQtd_Sabores"] != DBNull.Value
                                       ? Convert.ToInt32(r["mercQtd_Sabores"]) : 1,
                mercData_Cadastro    = r["mercData_Cadastro"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["mercData_Cadastro"]),
                Situacao             = r["Situacao"]?.ToString() ?? "A",
                Status_Transmissao   = r["Status_Transmissao"]?.ToString() ?? "N",
                Info                 = r["Info"]?.ToString() ?? "",
            };
        }
    }
}

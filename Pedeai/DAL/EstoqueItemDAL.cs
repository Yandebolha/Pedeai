using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class EstoqueItemDAL : BaseDAL
    {
        public DataTable Listar(string filtro = "")
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var where = string.IsNullOrWhiteSpace(filtro) ? "" : " AND estoNome LIKE @f";
            var sql = $@"SELECT Codigo, estoNome AS Nome, estoUnidade AS Unidade,
                               estoQtde_Atual AS Qtde, estoPreco_Custo AS Custo,
                               estoEh_Produto AS EhProduto, Situacao
                        FROM estoque_item
                        WHERE Situacao = 'A'{where}
                        ORDER BY estoNome";
            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(filtro))
                cmd.Parameters.AddWithValue("@f", "%" + filtro + "%");
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public EstoqueItem PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM estoque_item WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return Mapear(r);
        }

        public string Incluir(EstoqueItem obj)
        {
            try
            {
                using var conn = AbrirConexao();

                using (var chk = new MySqlCommand(
                    "SELECT COUNT(*) FROM estoque_item WHERE estoNome = @nome AND Situacao = 'A'", conn))
                {
                    chk.Parameters.AddWithValue("@nome", obj.estoNome);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return $"J\u00e1 existe um item de estoque com o nome '{obj.estoNome}'.";
                }

                using var cmd = new MySqlCommand(@"
                    INSERT INTO estoque_item
                        (estoNome, estoUnidade, estoQtde_Atual, estoPreco_Custo,
                         estoEh_Produto, estoFracao_Entrada, estoFracao_Entrada_Unidade,
                         estoFracao_Saida, estoFracao_Saida_Unidade, Codigo_Grupo,
                         Codigo_Mercadoria, Situacao, estoData_Cadastro)
                    VALUES (@nome, @un, @qtde, @custo, @ehprod, @fracent, @fracentun,
                            @fracsai, @fracsaiun, @codgrupo, @codmerc, 'A', NOW());
                    SELECT LAST_INSERT_ID();", conn);
                Bind(cmd, obj);
                obj.Codigo = Convert.ToInt32(cmd.ExecuteScalar());
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(EstoqueItem obj)
        {
            try
            {
                using var conn = AbrirConexao();

                using (var chk = new MySqlCommand(
                    "SELECT COUNT(*) FROM estoque_item WHERE estoNome = @nome AND Situacao = 'A' AND Codigo <> @cod", conn))
                {
                    chk.Parameters.AddWithValue("@nome", obj.estoNome);
                    chk.Parameters.AddWithValue("@cod",  obj.Codigo);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return $"J\u00e1 existe outro item com o nome '{obj.estoNome}'.";
                }

                using var cmd = new MySqlCommand(@"
                    UPDATE estoque_item SET
                        estoNome = @nome, estoUnidade = @un,
                        estoQtde_Atual = @qtde, estoPreco_Custo = @custo,
                        estoEh_Produto = @ehprod,
                        estoFracao_Entrada = @fracent, estoFracao_Entrada_Unidade = @fracentun,
                        estoFracao_Saida = @fracsai, estoFracao_Saida_Unidade = @fracsaiun,
                        Codigo_Grupo = @codgrupo, Codigo_Mercadoria = @codmerc
                    WHERE Codigo = @cod", conn);
                Bind(cmd, obj);
                cmd.Parameters.AddWithValue("@cod", obj.Codigo);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string AjustarQuantidade(int codigo, decimal delta)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd = new MySqlCommand(
                    "UPDATE estoque_item SET estoQtde_Atual = estoQtde_Atual + @delta WHERE Codigo = @cod", conn);
                cmd.Parameters.AddWithValue("@delta", delta);
                cmd.Parameters.AddWithValue("@cod",   codigo);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public void Desativar(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "UPDATE estoque_item SET Situacao = 'I' WHERE Codigo = @cod", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            cmd.ExecuteNonQuery();
        }

        private static void Bind(MySqlCommand cmd, EstoqueItem obj)
        {
            cmd.Parameters.AddWithValue("@nome",   obj.estoNome);
            cmd.Parameters.AddWithValue("@un",     obj.estoUnidade);
            cmd.Parameters.AddWithValue("@qtde",   obj.estoQtde_Atual);
            cmd.Parameters.AddWithValue("@custo",  obj.estoPreco_Custo);
            cmd.Parameters.AddWithValue("@ehprod",  obj.estoEh_Produto ? 1 : 0);
            cmd.Parameters.AddWithValue("@fracent",   obj.estoFracao_Entrada);
            cmd.Parameters.AddWithValue("@fracentun", obj.estoFracao_Entrada_Unidade ?? "");
            cmd.Parameters.AddWithValue("@fracsai",   obj.estoFracao_Saida);
            cmd.Parameters.AddWithValue("@fracsaiun", obj.estoFracao_Saida_Unidade ?? "");
            cmd.Parameters.AddWithValue("@codgrupo",
                (object)obj.Codigo_Grupo ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@codmerc",
                (object)obj.Codigo_Mercadoria ?? DBNull.Value);
        }

        private static EstoqueItem Mapear(MySqlDataReader r) => new EstoqueItem
        {
            Codigo            = Convert.ToInt32(r["Codigo"]),
            estoNome          = r["estoNome"]?.ToString() ?? "",
            estoUnidade       = r["estoUnidade"]?.ToString() ?? "un",
            estoQtde_Atual    = r["estoQtde_Atual"]  == DBNull.Value ? 0 : Convert.ToDecimal(r["estoQtde_Atual"]),
            estoPreco_Custo   = r["estoPreco_Custo"] == DBNull.Value ? 0 : Convert.ToDecimal(r["estoPreco_Custo"]),
            estoEh_Produto    = r["estoEh_Produto"]?.ToString() == "1" || r["estoEh_Produto"]?.ToString() == "True",
            estoFracao_Entrada          = r["estoFracao_Entrada"] == DBNull.Value ? 1 : Convert.ToDecimal(r["estoFracao_Entrada"]),
            estoFracao_Entrada_Unidade  = r["estoFracao_Entrada_Unidade"]?.ToString() ?? "",
            estoFracao_Saida            = r["estoFracao_Saida"]  == DBNull.Value ? 1 : Convert.ToDecimal(r["estoFracao_Saida"]),
            estoFracao_Saida_Unidade    = r["estoFracao_Saida_Unidade"]?.ToString() ?? "",
            Codigo_Grupo      = r["Codigo_Grupo"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["Codigo_Grupo"]),
            Codigo_Mercadoria = r["Codigo_Mercadoria"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["Codigo_Mercadoria"]),
            Situacao          = r["Situacao"]?.ToString() ?? "A",
            estoData_Cadastro = r["estoData_Cadastro"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["estoData_Cadastro"]),
        };
    }
}

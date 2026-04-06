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
                               estoEstoque_Min AS EstMinimo,
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
                using var cmd = new MySqlCommand(@"
                    INSERT INTO estoque_item
                        (estoNome, estoUnidade, estoQtde_Atual, estoPreco_Custo,
                         estoEstoque_Min, estoEh_Produto, Codigo_Mercadoria, Situacao, estoData_Cadastro)
                    VALUES (@nome, @un, @qtde, @custo, @min, @ehprod, @codmerc, 'A', NOW())", conn);
                Bind(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(EstoqueItem obj)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd = new MySqlCommand(@"
                    UPDATE estoque_item SET
                        estoNome = @nome, estoUnidade = @un,
                        estoQtde_Atual = @qtde, estoPreco_Custo = @custo,
                        estoEstoque_Min = @min, estoEh_Produto = @ehprod,
                        Codigo_Mercadoria = @codmerc
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
            cmd.Parameters.AddWithValue("@min",    obj.estoEstoque_Min);
            cmd.Parameters.AddWithValue("@ehprod", obj.estoEh_Produto ? 1 : 0);
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
            estoEstoque_Min   = r["estoEstoque_Min"] == DBNull.Value ? 0 : Convert.ToDecimal(r["estoEstoque_Min"]),
            estoEh_Produto    = r["estoEh_Produto"]?.ToString() == "1" || r["estoEh_Produto"]?.ToString() == "True",
            Codigo_Mercadoria = r["Codigo_Mercadoria"] == DBNull.Value ? (int?)null : Convert.ToInt32(r["Codigo_Mercadoria"]),
            Situacao          = r["Situacao"]?.ToString() ?? "A",
            estoData_Cadastro = r["estoData_Cadastro"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["estoData_Cadastro"]),
        };
    }
}

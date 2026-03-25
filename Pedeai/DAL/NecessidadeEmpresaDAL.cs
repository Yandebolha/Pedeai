using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    /// <summary>
    /// Acesso a dados para necessidades fixas da empresa (aluguel, energia, etc.).
    /// Requer tabela: CREATE TABLE necessidade_empresa (
    ///   auxCodigo INT, Codigo INT PRIMARY KEY,
    ///   nempData DATE, nempCategoria VARCHAR(50),
    ///   nempDescricao VARCHAR(200), nempValor DECIMAL(10,2),
    ///   Situacao CHAR(1) DEFAULT 'A', Info TEXT DEFAULT '');
    /// </summary>
    public class NecessidadeEmpresaDAL : BaseDAL
    {
        public DataTable Listar(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            const string sql = @"
                SELECT Codigo, nempData AS Data, nempCategoria AS Categoria,
                       nempDescricao AS Descricao, nempValor AS Valor
                FROM necessidade_empresa
                WHERE nempData BETWEEN @de AND @ate AND Situacao='A'
                ORDER BY nempData DESC, Codigo DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public decimal TotalPeriodo(DateTime de, DateTime ate)
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "SELECT COALESCE(SUM(nempValor),0) FROM necessidade_empresa WHERE nempData BETWEEN @de AND @ate AND Situacao='A'",
                conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            var r = cmd.ExecuteScalar();
            return r == null || r == DBNull.Value ? 0m : Convert.ToDecimal(r);
        }

        public string Inserir(NecessidadeEmpresa obj)
        {
            try
            {
                using var conn = AbrirConexao();
                obj.Codigo    = ProximoCodigo("necessidade_empresa", conn);
                obj.auxCodigo = ProximoAuxCodigo("necessidade_empresa", conn);
                const string sql = @"
                    INSERT INTO necessidade_empresa
                        (auxCodigo, Codigo, nempData, nempCategoria, nempDescricao, nempValor, Situacao, Info)
                    VALUES(@aux, @cod, @data, @cat, @desc, @val, 'A', '')";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@aux",  obj.auxCodigo);
                cmd.Parameters.AddWithValue("@cod",  obj.Codigo);
                cmd.Parameters.AddWithValue("@data", obj.nempData.Date);
                cmd.Parameters.AddWithValue("@cat",  obj.nempCategoria ?? "");
                cmd.Parameters.AddWithValue("@desc", obj.nempDescricao ?? "");
                cmd.Parameters.AddWithValue("@val",  obj.nempValor);
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
                using var cmd  = new MySqlCommand(
                    "UPDATE necessidade_empresa SET Situacao='I' WHERE Codigo=@cod", conn);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }
    }
}

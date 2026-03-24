using System;
using System.Configuration;
using MySqlConnector;

namespace Pedeai.DAL
{
    /// <summary>
    /// Classe base da camada de acesso a dados — padrão ConstruFarma.
    /// Fornece conexão, transação e geração de código sequencial.
    /// </summary>
    public class BaseDAL
    {
        protected static string StrConexao =>
            ConfigurationManager.AppSettings["ConnectionString"]
            ?? "Server=localhost;Database=pedeai;User=root;Password=;Port=3306;CharSet=utf8mb4;";

        /// <summary>Abre e retorna uma conexão MySQL.</summary>
        protected MySqlConnection AbrirConexao()
        {
            var conn = new MySqlConnection(StrConexao);
            conn.Open();
            return conn;
        }

        /// <summary>
        /// Retorna o próximo Codigo disponível (MAX + 1) para a tabela informada.
        /// Padrão ConstruFarma: PK gerenciada pela aplicação, não pelo banco.
        /// </summary>
        protected int ProximoCodigo(string tabela, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            bool deveFecchar = conn == null;
            if (conn == null) conn = AbrirConexao();
            try
            {
                using var cmd = new MySqlCommand(
                    "SELECT COALESCE(MAX(Codigo),0)+1 FROM `" + tabela + "`", conn);
                if (trans != null) cmd.Transaction = trans;
                var result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 1 : Convert.ToInt32(result);
            }
            finally
            {
                if (deveFecchar) conn.Dispose();
            }
        }

        /// <summary>
        /// Retorna o próximo auxCodigo disponível (MAX + 1) para a tabela.
        /// Padrão ConstruFarma: auxCodigo = identificador na origem.
        /// </summary>
        protected int ProximoAuxCodigo(string tabela, MySqlConnection conn = null, MySqlTransaction trans = null)
        {
            bool deveFecchar = conn == null;
            if (conn == null) conn = AbrirConexao();
            try
            {
                using var cmd = new MySqlCommand(
                    "SELECT COALESCE(MAX(auxCodigo),0)+1 FROM `" + tabela + "`", conn);
                if (trans != null) cmd.Transaction = trans;
                var result = cmd.ExecuteScalar();
                return result == null || result == DBNull.Value ? 1 : Convert.ToInt32(result);
            }
            finally
            {
                if (deveFecchar) conn.Dispose();
            }
        }
    }
}

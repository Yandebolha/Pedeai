using System.Configuration;
using MySqlConnector;

namespace Pedeai.DB
{
    /// <summary>Auxiliar de conexão MySQL — utilizado somente para testar a conectividade na inicialização.</summary>
    public static class DbHelper
    {
        public static string ConnectionString =>
            ConfigurationManager.AppSettings["ConnectionString"]
            ?? "Server=localhost;Database=pedeai;User=root;Password=;Port=3306;CharSet=utf8mb4;SslMode=None;";

        private static MySqlConnection AbrirConexao()
        {
            var conn = new MySqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        /// <summary>Testa se o servidor MySQL está acessível (sem exigir que o banco já exista).</summary>
        public static bool TestarConexao()
        {
            try
            {
                var b = new MySqlConnector.MySqlConnectionStringBuilder(ConnectionString);
                b.Database = "";
                using var conn = new MySqlConnector.MySqlConnection(b.ToString());
                conn.Open();
                return true;
            }
            catch { return false; }
        }
    }
}

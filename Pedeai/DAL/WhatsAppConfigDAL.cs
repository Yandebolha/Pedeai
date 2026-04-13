using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class WhatsAppConfigDAL : BaseDAL
    {
        public WhatsAppConfig Carregar()
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT whaApiUrl, whaApiKey, whaInstance, whaMsgPreparo, whaMsgEntrega, whaMsgCupom " +
                "FROM config_whatsapp WHERE Codigo=1 LIMIT 1", conn);
            using var rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                return new WhatsAppConfig
                {
                    ApiUrl     = rd.IsDBNull(rd.GetOrdinal("whaApiUrl"))     ? "" : rd.GetString("whaApiUrl"),
                    ApiKey     = rd.IsDBNull(rd.GetOrdinal("whaApiKey"))     ? "" : rd.GetString("whaApiKey"),
                    Instance   = rd.IsDBNull(rd.GetOrdinal("whaInstance"))   ? "pedeai" : rd.GetString("whaInstance"),
                    MsgPreparo = rd.IsDBNull(rd.GetOrdinal("whaMsgPreparo")) ? "" : rd.GetString("whaMsgPreparo"),
                    MsgEntrega = rd.IsDBNull(rd.GetOrdinal("whaMsgEntrega")) ? "" : rd.GetString("whaMsgEntrega"),
                    MsgCupom   = rd.IsDBNull(rd.GetOrdinal("whaMsgCupom"))   ? "" : rd.GetString("whaMsgCupom"),
                };
            }
            return new WhatsAppConfig();
        }

        public void Salvar(WhatsAppConfig cfg)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(@"
                INSERT INTO config_whatsapp
                    (Codigo, whaApiUrl, whaApiKey, whaInstance, whaMsgPreparo, whaMsgEntrega, whaMsgCupom)
                VALUES (1, @url, @key, @inst, @prep, @entr, @cup)
                ON DUPLICATE KEY UPDATE
                    whaApiUrl=@url, whaApiKey=@key, whaInstance=@inst,
                    whaMsgPreparo=@prep, whaMsgEntrega=@entr, whaMsgCupom=@cup", conn);
            cmd.Parameters.AddWithValue("@url",  cfg.ApiUrl     ?? "");
            cmd.Parameters.AddWithValue("@key",  cfg.ApiKey     ?? "");
            cmd.Parameters.AddWithValue("@inst", string.IsNullOrWhiteSpace(cfg.Instance) ? "pedeai" : cfg.Instance);
            cmd.Parameters.AddWithValue("@prep", cfg.MsgPreparo ?? "");
            cmd.Parameters.AddWithValue("@entr", cfg.MsgEntrega ?? "");
            cmd.Parameters.AddWithValue("@cup",  cfg.MsgCupom   ?? "");
            cmd.ExecuteNonQuery();
        }
    }
}

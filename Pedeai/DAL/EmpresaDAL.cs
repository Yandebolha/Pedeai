using System;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class EmpresaDAL : BaseDAL
    {
        // ── Leitura (registro único) ─────────────────────────────────────────
        public Empresa Carregar()
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "SELECT * FROM empresa ORDER BY Codigo LIMIT 1", conn);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return new Empresa { Codigo = 1 };
            return new Empresa
            {
                Codigo           = Convert.ToInt32(r["Codigo"]),
                empNome          = r["empNome"]?.ToString() ?? "",
                empNome_Fantasia = r["empNome_Fantasia"]?.ToString() ?? "",
                empCNPJ          = r["empCNPJ"]?.ToString() ?? "",
                empTelefone      = r["empTelefone"]?.ToString() ?? "",
                empEmail         = r["empEmail"]?.ToString() ?? "",
                empEndereco      = r["empEndereco"]?.ToString() ?? "",
                Info             = r["Info"]?.ToString() ?? "",
                empCodigo_Empresa = TryGet(r, "empCodigo_Empresa"),
                empChave_Licenca  = TryGet(r, "empChave_Licenca"),
                empData_Graca     = TryGetDate(r, "empData_Graca"),
            };
        }

        private static string TryGet(MySqlDataReader r, string col)
        {
            try { return r[col]?.ToString() ?? ""; } catch { return ""; }
        }

        private static DateTime? TryGetDate(MySqlDataReader r, string col)
        {
            try
            {
                var v = r[col];
                if (v == null || v == DBNull.Value) return null;
                return Convert.ToDateTime(v);
            }
            catch { return null; }
        }

        // ── Salvar (insert ou update) ────────────────────────────────────────
        public string Salvar(Empresa obj)
        {
            try
            {
                using var conn = AbrirConexao();
                using var chk  = new MySqlCommand(
                    "SELECT COUNT(*) FROM empresa WHERE Codigo=@cod", conn);
                chk.Parameters.AddWithValue("@cod", obj.Codigo);
                bool existe = Convert.ToInt32(chk.ExecuteScalar()) > 0;

                if (!existe)
                {
                    if (obj.Codigo <= 0) obj.Codigo = 1;
                    const string ins = @"
                        INSERT INTO empresa (Codigo,empNome,empNome_Fantasia,empCNPJ,empTelefone,empEmail,empEndereco,Info)
                        VALUES(@cod,@nome,@fant,@cnpj,@tel,@email,@end,@info)";
                    using var ins2 = new MySqlCommand(ins, conn);
                    ins2.Parameters.AddWithValue("@cod",   obj.Codigo);
                    ins2.Parameters.AddWithValue("@nome",  obj.empNome);
                    ins2.Parameters.AddWithValue("@fant",  obj.empNome_Fantasia ?? "");
                    ins2.Parameters.AddWithValue("@cnpj",  obj.empCNPJ ?? "");
                    ins2.Parameters.AddWithValue("@tel",   obj.empTelefone ?? "");
                    ins2.Parameters.AddWithValue("@email", obj.empEmail ?? "");
                    ins2.Parameters.AddWithValue("@end",   obj.empEndereco ?? "");
                    ins2.Parameters.AddWithValue("@info",  obj.Info ?? "");
                    ins2.ExecuteNonQuery();
                }
                else
                {
                    const string upd = @"
                        UPDATE empresa SET empNome=@nome,empNome_Fantasia=@fant,empCNPJ=@cnpj,
                            empTelefone=@tel,empEmail=@email,empEndereco=@end,Info=@info
                        WHERE Codigo=@cod";
                    using var upd2 = new MySqlCommand(upd, conn);
                    upd2.Parameters.AddWithValue("@nome",  obj.empNome);
                    upd2.Parameters.AddWithValue("@fant",  obj.empNome_Fantasia ?? "");
                    upd2.Parameters.AddWithValue("@cnpj",  obj.empCNPJ ?? "");
                    upd2.Parameters.AddWithValue("@tel",   obj.empTelefone ?? "");
                    upd2.Parameters.AddWithValue("@email", obj.empEmail ?? "");
                    upd2.Parameters.AddWithValue("@end",   obj.empEndereco ?? "");
                    upd2.Parameters.AddWithValue("@info",  obj.Info ?? "");
                    upd2.Parameters.AddWithValue("@cod",   obj.Codigo);
                    upd2.ExecuteNonQuery();
                }
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Salva apenas a chave de licença e reseta o período de graça.</summary>
        public void SalvarLicenca(int codEmpresa, string chave)
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "UPDATE empresa SET empChave_Licenca=@chave, empData_Graca=NULL WHERE Codigo=@cod", conn);
            cmd.Parameters.AddWithValue("@chave", chave ?? "");
            cmd.Parameters.AddWithValue("@cod",   codEmpresa);
            cmd.ExecuteNonQuery();
        }

        /// <summary>Define a data de início do período de graça.</summary>
        public void SalvarDataGraca(int codEmpresa, DateTime data)
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "UPDATE empresa SET empData_Graca=@data WHERE Codigo=@cod", conn);
            cmd.Parameters.AddWithValue("@data", data.Date);
            cmd.Parameters.AddWithValue("@cod",  codEmpresa);
            cmd.ExecuteNonQuery();
        }
    }
}

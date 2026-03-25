using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class UsuarioDAL : BaseDAL
    {
        // ── Hash de senha (SHA-256) ──────────────────────────────────────────
        public static string HashSenha(string senha)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(senha ?? ""));
            return Convert.ToHexString(bytes).ToLower();
        }

        // ── Autenticação ─────────────────────────────────────────────────────
        public Usuario Autenticar(string login, string senha)
        {
            var hash = HashSenha(senha);
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM usuario WHERE usuLogin=@l AND usuSenha=@s AND Situacao='A' LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@l", login);
            cmd.Parameters.AddWithValue("@s", hash);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return Mapear(r);
        }

        // ── Listagem ─────────────────────────────────────────────────────────
        public List<Usuario> Listar()
        {
            var lista = new List<Usuario>();
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM usuario ORDER BY usuNome", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(Mapear(r));
            return lista;
        }

        public Usuario PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM usuario WHERE Codigo=@cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            return r.Read() ? Mapear(r) : null;
        }

        // ── Incluir ──────────────────────────────────────────────────────────
        public string Inserir(Usuario obj)
        {
            try
            {
                using var conn = AbrirConexao();
                obj.Codigo    = ProximoCodigo("usuario", conn);
                obj.auxCodigo = ProximoAuxCodigo("usuario", conn);
                const string sql = @"
                    INSERT INTO usuario
                        (auxCodigo,Codigo,usuNome,usuLogin,usuSenha,usuNivel,Situacao,usuData_Cadastro,Info)
                    VALUES
                        (@aux,@cod,@nome,@login,@senha,@nivel,'A',NOW(),@info)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@aux",   obj.auxCodigo);
                cmd.Parameters.AddWithValue("@cod",   obj.Codigo);
                cmd.Parameters.AddWithValue("@nome",  obj.usuNome);
                cmd.Parameters.AddWithValue("@login", obj.usuLogin);
                cmd.Parameters.AddWithValue("@senha", HashSenha(obj.usuSenha));
                cmd.Parameters.AddWithValue("@nivel", obj.usuNivel);
                cmd.Parameters.AddWithValue("@info",  obj.Info ?? "");
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        // ── Alterar ──────────────────────────────────────────────────────────
        /// <param name="alterarSenha">true se a senha nova foi preenchida e deve ser gravada.</param>
        public string Alterar(Usuario obj, bool alterarSenha)
        {
            try
            {
                using var conn = AbrirConexao();
                var sqlBase = alterarSenha
                    ? "UPDATE usuario SET usuNome=@nome,usuLogin=@login,usuSenha=@senha,usuNivel=@nivel,Situacao=@sit WHERE Codigo=@cod"
                    : "UPDATE usuario SET usuNome=@nome,usuLogin=@login,usuNivel=@nivel,Situacao=@sit WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sqlBase, conn);
                cmd.Parameters.AddWithValue("@nome",  obj.usuNome);
                cmd.Parameters.AddWithValue("@login", obj.usuLogin);
                if (alterarSenha)
                    cmd.Parameters.AddWithValue("@senha", HashSenha(obj.usuSenha));
                cmd.Parameters.AddWithValue("@nivel", obj.usuNivel);
                cmd.Parameters.AddWithValue("@sit",   obj.Situacao ?? "A");
                cmd.Parameters.AddWithValue("@cod",   obj.Codigo);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        // ── Mapeamento ───────────────────────────────────────────────────────
        private static Usuario Mapear(MySqlDataReader r) => new Usuario
        {
            Codigo           = Convert.ToInt32(r["Codigo"]),
            auxCodigo        = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
            usuNome          = r["usuNome"]?.ToString() ?? "",
            usuLogin         = r["usuLogin"]?.ToString() ?? "",
            usuSenha         = r["usuSenha"]?.ToString() ?? "",
            usuNivel         = r["usuNivel"] == DBNull.Value ? 1 : Convert.ToInt32(r["usuNivel"]),
            Situacao         = r["Situacao"]?.ToString() ?? "A",
            usuData_Cadastro = r["usuData_Cadastro"] == DBNull.Value
                                    ? DateTime.Now
                                    : Convert.ToDateTime(r["usuData_Cadastro"]),
            Info             = r["Info"]?.ToString() ?? "",
        };
    }
}

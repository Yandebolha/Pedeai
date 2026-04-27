using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class GrupoMercadoriaDAL : BaseDAL
    {
        public DataTable Listar(bool apenasAtivas = false)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = "SELECT Codigo, grmeDescricao_ AS Nome, grmeOrdem AS Ordem, Situacao FROM grupo_mercadoria"
                    + (apenasAtivas ? " WHERE Situacao='A'" : "")
                    + " ORDER BY grmeOrdem, grmeDescricao_";
            using var cmd = new MySqlCommand(sql, conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public GrupoMercadoria PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM grupo_mercadoria WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new GrupoMercadoria
            {
                Codigo             = Convert.ToInt32(r["Codigo"]),
                auxCodigo          = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
                grmeDescricao_     = r["grmeDescricao_"]?.ToString() ?? "",
                grmeOrdem          = r["grmeOrdem"] == DBNull.Value ? 0 : Convert.ToInt32(r["grmeOrdem"]),
                grmeHabilitar_Site = TryGetBool(r, "grmeHabilitar_Site"),
                grmeImagem_Url     = TryGetStr(r, "grmeImagem_Url"),
                grmeData_Cadastro  = r["grmeData_Cadastro"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["grmeData_Cadastro"]),
                Situacao           = r["Situacao"]?.ToString() ?? "A",
                Status_Transmissao = r["Status_Transmissao"]?.ToString() ?? "N",
                Info               = r["Info"]?.ToString() ?? "",
            };
        }

        public string Incluir(GrupoMercadoria obj)
        {
            try
            {
                using var conn = AbrirConexao();
                obj.Codigo    = ProximoCodigo("grupo_mercadoria", conn);
                obj.auxCodigo = ProximoAuxCodigo("grupo_mercadoria", conn);
                obj.grmeData_Cadastro = DateTime.Now;

                var sql = @"INSERT INTO grupo_mercadoria
                            (auxCodigo, Codigo, grmeDescricao_, grmeOrdem, grmeHabilitar_Site, grmeImagem_Url, Situacao, Status_Transmissao, Info, grmeData_Cadastro)
                            VALUES(@aux, @cod, @nome, @ordem, @habSite, @imgUrl, @sit, @trans, @info, @dt)";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(GrupoMercadoria obj)
        {
            try
            {
                using var conn = AbrirConexao();
                var sql = "UPDATE grupo_mercadoria SET grmeDescricao_=@nome, grmeOrdem=@ordem, grmeHabilitar_Site=@habSite, grmeImagem_Url=@imgUrl, Situacao=@sit WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private static void BindParams(MySqlCommand cmd, GrupoMercadoria obj)
        {
            cmd.Parameters.AddWithValue("@aux",     obj.auxCodigo);
            cmd.Parameters.AddWithValue("@cod",     obj.Codigo);
            cmd.Parameters.AddWithValue("@nome",    obj.grmeDescricao_ ?? "");
            cmd.Parameters.AddWithValue("@ordem",   obj.grmeOrdem);
            cmd.Parameters.AddWithValue("@habSite", obj.grmeHabilitar_Site ? 1 : 0);
            cmd.Parameters.AddWithValue("@imgUrl",  obj.grmeImagem_Url ?? "");
            cmd.Parameters.AddWithValue("@sit",     obj.Situacao ?? "A");
            cmd.Parameters.AddWithValue("@trans",   obj.Status_Transmissao ?? "N");
            cmd.Parameters.AddWithValue("@info",    obj.Info ?? "");
            cmd.Parameters.AddWithValue("@dt",      obj.grmeData_Cadastro);
        }

        private static bool TryGetBool(MySqlDataReader r, string col)
        { try { return r[col] != DBNull.Value && Convert.ToBoolean(r[col]); } catch { return false; } }
        private static string TryGetStr(MySqlDataReader r, string col)
        { try { return r[col]?.ToString() ?? ""; } catch { return ""; } }
    }
}

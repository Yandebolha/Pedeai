using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class BairroDAL : BaseDAL
    {
        public DataTable Listar(bool apenasAtivos = false)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = "SELECT Codigo, baiCidade AS Cidade, baiNome AS Bairro, baiTaxa_Entrega AS Taxa, Situacao " +
                      "FROM bairro" +
                      (apenasAtivos ? " WHERE Situacao='A'" : "") +
                      " ORDER BY baiCidade, baiNome";
            using var cmd = new MySqlCommand(sql, conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public Bairro PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM bairro WHERE Codigo=@cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return Mapear(r);
        }

        public Bairro BuscarPorNome(string cidade, string bairro)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM bairro WHERE Situacao='A' AND baiCidade=@cid AND baiNome=@nom LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cid", cidade ?? "");
            cmd.Parameters.AddWithValue("@nom", bairro ?? "");
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return Mapear(r);
        }

        public string Incluir(Bairro obj)
        {
            try
            {
                using var conn = AbrirConexao();
                obj.Codigo    = ProximoCodigo("bairro", conn);
                obj.auxCodigo = 1;
                using var cmd = new MySqlCommand(
                    "INSERT INTO bairro (Codigo, auxCodigo, baiCidade, baiNome, baiTaxa_Entrega, Situacao) " +
                    "VALUES(@cod, @aux, @cid, @nom, @taxa, @sit)", conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(Bairro obj)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd = new MySqlCommand(
                    "UPDATE bairro SET baiCidade=@cid, baiNome=@nom, baiTaxa_Entrega=@taxa, Situacao=@sit " +
                    "WHERE Codigo=@cod", conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private static void BindParams(MySqlCommand cmd, Bairro obj)
        {
            cmd.Parameters.AddWithValue("@cod",  obj.Codigo);
            cmd.Parameters.AddWithValue("@aux",  obj.auxCodigo);
            cmd.Parameters.AddWithValue("@cid",  obj.baiCidade ?? "");
            cmd.Parameters.AddWithValue("@nom",  obj.baiNome ?? "");
            cmd.Parameters.AddWithValue("@taxa", obj.baiTaxa_Entrega);
            cmd.Parameters.AddWithValue("@sit",  obj.Situacao ?? "A");
        }

        private static Bairro Mapear(MySqlDataReader r) => new Bairro
        {
            Codigo          = Convert.ToInt32(r["Codigo"]),
            auxCodigo       = r["auxCodigo"] == DBNull.Value ? 1 : Convert.ToInt32(r["auxCodigo"]),
            baiCidade       = r["baiCidade"]?.ToString() ?? "",
            baiNome         = r["baiNome"]?.ToString() ?? "",
            baiTaxa_Entrega = r["baiTaxa_Entrega"] == DBNull.Value ? 0 : Convert.ToDecimal(r["baiTaxa_Entrega"]),
            Situacao        = r["Situacao"]?.ToString() ?? "A",
        };
    }
}

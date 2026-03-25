using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class GastoMaterialDAL : BaseDAL
    {
        // ── Listar ───────────────────────────────────────────────────────────
        public DataTable Listar(DateTime de, DateTime ate)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            const string sql = @"
                SELECT Codigo, gmatData AS Data, gmatDescricao AS Descricao,
                       gmatValor AS Valor, gmatObservacoes AS Observacoes
                FROM gasto_material
                WHERE gmatData BETWEEN @de AND @ate AND Situacao='A'
                ORDER BY gmatData DESC, Codigo DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public List<GastoMaterial> ListarObjetos(DateTime de, DateTime ate)
        {
            var lista = new List<GastoMaterial>();
            using var conn = AbrirConexao();
            const string sql = @"SELECT * FROM gasto_material
                                  WHERE gmatData BETWEEN @de AND @ate AND Situacao='A'
                                  ORDER BY gmatData DESC";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            using var r = cmd.ExecuteReader();
            while (r.Read()) lista.Add(Mapear(r));
            return lista;
        }

        // ── Total de gastos por período ──────────────────────────────────────
        public decimal TotalPeriodo(DateTime de, DateTime ate)
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "SELECT COALESCE(SUM(gmatValor),0) FROM gasto_material WHERE gmatData BETWEEN @de AND @ate AND Situacao='A'",
                conn);
            cmd.Parameters.AddWithValue("@de",  de.Date);
            cmd.Parameters.AddWithValue("@ate", ate.Date);
            var r = cmd.ExecuteScalar();
            return r == null || r == DBNull.Value ? 0m : Convert.ToDecimal(r);
        }

        // ── Inserir ──────────────────────────────────────────────────────────
        public string Inserir(GastoMaterial obj)
        {
            try
            {
                using var conn = AbrirConexao();
                obj.Codigo    = ProximoCodigo("gasto_material", conn);
                obj.auxCodigo = ProximoAuxCodigo("gasto_material", conn);
                const string sql = @"
                    INSERT INTO gasto_material
                        (auxCodigo,Codigo,gmatData,gmatDescricao,gmatValor,gmatObservacoes,Situacao,Info)
                    VALUES(@aux,@cod,@data,@desc,@val,@obs,'A',@info)";
                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@aux",  obj.auxCodigo);
                cmd.Parameters.AddWithValue("@cod",  obj.Codigo);
                cmd.Parameters.AddWithValue("@data", obj.gmatData.Date);
                cmd.Parameters.AddWithValue("@desc", obj.gmatDescricao ?? "");
                cmd.Parameters.AddWithValue("@val",  obj.gmatValor);
                cmd.Parameters.AddWithValue("@obs",  obj.gmatObservacoes ?? "");
                cmd.Parameters.AddWithValue("@info", obj.Info ?? "");
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        // ── Excluir (inativar) ───────────────────────────────────────────────
        public string Excluir(int codigo)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd  = new MySqlCommand(
                    "UPDATE gasto_material SET Situacao='I' WHERE Codigo=@cod", conn);
                cmd.Parameters.AddWithValue("@cod", codigo);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private static GastoMaterial Mapear(MySqlDataReader r) => new GastoMaterial
        {
            Codigo          = Convert.ToInt32(r["Codigo"]),
            auxCodigo       = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
            gmatData        = r["gmatData"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(r["gmatData"]),
            gmatDescricao   = r["gmatDescricao"]?.ToString() ?? "",
            gmatValor       = r["gmatValor"] == DBNull.Value ? 0 : Convert.ToDecimal(r["gmatValor"]),
            gmatObservacoes = r["gmatObservacoes"]?.ToString() ?? "",
            Situacao        = r["Situacao"]?.ToString() ?? "A",
            Info            = r["Info"]?.ToString() ?? "",
        };
    }
}

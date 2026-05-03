using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class MarmitaDAL : BaseDAL
    {
        // ── Marmita ────────────────────────────────────────────────────────────

        public DataTable Listar(bool apenasAtivas = false)
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            string where = apenasAtivas ? "WHERE Situacao = 'A'" : "";
            var sql = $"SELECT Codigo, marDescricao AS Descricao, marValor AS `Valor R$`, marCusto AS `Custo R$`, Situacao FROM marmita {where} ORDER BY marDescricao";
            using var cmd = new MySqlCommand(sql, conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public Marmita PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand("SELECT * FROM marmita WHERE Codigo=@c LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@c", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return new Marmita
            {
                Codigo           = Convert.ToInt32(r["Codigo"]),
                auxCodigo        = Convert.ToInt32(r["auxCodigo"]),
                marDescricao     = r["marDescricao"]?.ToString() ?? "",
                marValor         = Convert.ToDecimal(r["marValor"]),
                marCusto         = r["marCusto"] == DBNull.Value ? 0m : Convert.ToDecimal(r["marCusto"]),
                marHabilitar_Site = r["marHabilitar_Site"] != DBNull.Value && Convert.ToBoolean(r["marHabilitar_Site"]),
                marDestaque      = r["marDestaque"] != DBNull.Value && Convert.ToBoolean(r["marDestaque"]),
                marImagem_Url    = TryGetString(r, "marImagem_Url"),
                supabase_uuid    = TryGetString(r, "supabase_uuid"),
                Situacao         = (r["Situacao"]?.ToString() ?? "A")[0],
                marMaxComplementos = TryGetInt(r, "marMaxComplementos", 1)
            };
        }

        private static string TryGetString(MySqlDataReader r, string col)
        {
            try { return r[col]?.ToString() ?? ""; } catch { return ""; }
        }

        private static int TryGetInt(MySqlDataReader r, string col, int def = 0)
        {
            try { return r[col] == DBNull.Value ? def : Convert.ToInt32(r[col]); } catch { return def; }
        }

        private static string TryGetString2(MySqlDataReader r, string col, string def = "")
        {
            try { var v = r[col]; return v == DBNull.Value ? def : (v?.ToString() ?? def); } catch { return def; }
        }

        private static int TryGetInt2(MySqlDataReader r, string col, int def = 0)
        {
            try { return r[col] == DBNull.Value ? def : Convert.ToInt32(r[col]); } catch { return def; }
        }

        public string Incluir(Marmita obj)
        {
            try
            {
                using var conn = AbrirConexao();
                obj.Codigo    = ProximoCodigo("marmita", conn);
                obj.auxCodigo = ProximoAuxCodigo("marmita", conn);
                using var cmd = new MySqlCommand(@"
                    INSERT INTO marmita (Codigo, auxCodigo, marDescricao, marValor, marCusto,
                                        marHabilitar_Site, marDestaque, marImagem_Url, Situacao, marMaxComplementos)
                    VALUES (@cod, @aux, @desc, @val, @cst, @site, @dest, @img, @sit, @maxcomp)", conn);
                cmd.Parameters.AddWithValue("@cod",     obj.Codigo);
                cmd.Parameters.AddWithValue("@aux",     obj.auxCodigo);
                cmd.Parameters.AddWithValue("@desc",    obj.marDescricao);
                cmd.Parameters.AddWithValue("@val",     obj.marValor);
                cmd.Parameters.AddWithValue("@cst",     obj.marCusto);
                cmd.Parameters.AddWithValue("@site",    obj.marHabilitar_Site ? 1 : 0);
                cmd.Parameters.AddWithValue("@dest",    obj.marDestaque ? 1 : 0);
                cmd.Parameters.AddWithValue("@img",     obj.marImagem_Url ?? "");
                cmd.Parameters.AddWithValue("@sit",     obj.Situacao.ToString());
                cmd.Parameters.AddWithValue("@maxcomp", obj.marMaxComplementos < 1 ? 1 : obj.marMaxComplementos);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(Marmita obj)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd = new MySqlCommand(@"
                    UPDATE marmita SET marDescricao=@desc, marValor=@val, marCusto=@cst,
                                      marHabilitar_Site=@site, marDestaque=@dest,
                                      marImagem_Url=@img, Situacao=@sit, marMaxComplementos=@maxcomp
                    WHERE Codigo=@cod", conn);
                cmd.Parameters.AddWithValue("@desc",    obj.marDescricao);
                cmd.Parameters.AddWithValue("@val",     obj.marValor);
                cmd.Parameters.AddWithValue("@cst",     obj.marCusto);
                cmd.Parameters.AddWithValue("@site",    obj.marHabilitar_Site ? 1 : 0);
                cmd.Parameters.AddWithValue("@dest",    obj.marDestaque ? 1 : 0);
                cmd.Parameters.AddWithValue("@img",     obj.marImagem_Url ?? "");
                cmd.Parameters.AddWithValue("@sit",     obj.Situacao.ToString());
                cmd.Parameters.AddWithValue("@maxcomp", obj.marMaxComplementos < 1 ? 1 : obj.marMaxComplementos);
                cmd.Parameters.AddWithValue("@cod",     obj.Codigo);
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
                using var t = conn.BeginTransaction();
                using (var cmd = new MySqlCommand("DELETE FROM marmita_item WHERE Codigo_Marmita=@c", conn, t))
                { cmd.Parameters.AddWithValue("@c", codigo); cmd.ExecuteNonQuery(); }
                using (var cmd = new MySqlCommand("DELETE FROM marmita WHERE Codigo=@c", conn, t))
                { cmd.Parameters.AddWithValue("@c", codigo); cmd.ExecuteNonQuery(); }
                t.Commit();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        // ── Migrations ───────────────────────────────────────────────────────

        public void EnsureMigrations()
        {
            using var conn = AbrirConexao();
            // maritmGrupo
            using (var check = new MySqlCommand(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='marmita_item' AND COLUMN_NAME='maritmGrupo'", conn))
            {
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    using var alter = new MySqlCommand(
                        "ALTER TABLE marmita_item ADD COLUMN maritmGrupo VARCHAR(100) NOT NULL DEFAULT 'Geral'", conn);
                    alter.ExecuteNonQuery();
                }
            }
            // maritmGrupoMax
            using (var check = new MySqlCommand(
                "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='marmita_item' AND COLUMN_NAME='maritmGrupoMax'", conn))
            {
                if (Convert.ToInt32(check.ExecuteScalar()) == 0)
                {
                    using var alter = new MySqlCommand(
                        "ALTER TABLE marmita_item ADD COLUMN maritmGrupoMax INT NOT NULL DEFAULT 1", conn);
                    alter.ExecuteNonQuery();
                }
            }
        }

        // ── MarmitaItem ───────────────────────────────────────────────────────

        public List<MarmitaItem> ListarItens(int codigoMarmita)
        {
            var list = new List<MarmitaItem>();
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM marmita_item WHERE Codigo_Marmita=@c ORDER BY maritmNome", conn);
            cmd.Parameters.AddWithValue("@c", codigoMarmita);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                list.Add(new MarmitaItem
                {
                    Codigo            = Convert.ToInt32(r["Codigo"]),
                    auxCodigo         = Convert.ToInt32(r["auxCodigo"]),
                    Codigo_Marmita    = Convert.ToInt32(r["Codigo_Marmita"]),
                    maritmCodigo_Merc = Convert.ToInt32(r["maritmCodigo_Merc"]),
                    maritmNome        = r["maritmNome"]?.ToString() ?? "",
                    maritmQtde        = Convert.ToDecimal(r["maritmQtde"]),
                    maritmGrupo       = TryGetString2(r, "maritmGrupo", "Geral"),
                    maritmGrupoMax    = TryGetInt2(r, "maritmGrupoMax", 1)
                });
            return list;
        }

        public string AdicionarItem(MarmitaItem item)
        {
            try
            {
                using var conn = AbrirConexao();
                item.Codigo    = ProximoCodigo("marmita_item", conn);
                item.auxCodigo = ProximoAuxCodigo("marmita_item", conn);
                using var cmd = new MySqlCommand(@"
                    INSERT INTO marmita_item (Codigo, auxCodigo, Codigo_Marmita, maritmCodigo_Merc, maritmNome, maritmQtde, maritmGrupo, maritmGrupoMax)
                    VALUES (@cod, @aux, @mar, @merc, @nome, @qtde, @grupo, @maxg)", conn);
                cmd.Parameters.AddWithValue("@cod",   item.Codigo);
                cmd.Parameters.AddWithValue("@aux",   item.auxCodigo);
                cmd.Parameters.AddWithValue("@mar",   item.Codigo_Marmita);
                cmd.Parameters.AddWithValue("@merc",  item.maritmCodigo_Merc);
                cmd.Parameters.AddWithValue("@nome",  item.maritmNome);
                cmd.Parameters.AddWithValue("@qtde",  item.maritmQtde);
                cmd.Parameters.AddWithValue("@grupo", string.IsNullOrWhiteSpace(item.maritmGrupo) ? "Geral" : item.maritmGrupo);
                cmd.Parameters.AddWithValue("@maxg",  item.maritmGrupoMax < 1 ? 1 : item.maritmGrupoMax);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string RemoverItem(int codigoItem)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd = new MySqlCommand("DELETE FROM marmita_item WHERE Codigo=@c", conn);
                cmd.Parameters.AddWithValue("@c", codigoItem);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Atualiza maritmGrupoMax de todos os itens de um grupo específico em uma marmita.</summary>
        public string AtualizarGrupoMax(int codigoMarmita, string grupo, int max)
        {
            try
            {
                using var conn = AbrirConexao();
                using var cmd = new MySqlCommand(
                    "UPDATE marmita_item SET maritmGrupoMax=@max WHERE Codigo_Marmita=@mar AND maritmGrupo=@grp", conn);
                cmd.Parameters.AddWithValue("@max", max < 1 ? 1 : max);
                cmd.Parameters.AddWithValue("@mar", codigoMarmita);
                cmd.Parameters.AddWithValue("@grp", grupo);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }
    }
}

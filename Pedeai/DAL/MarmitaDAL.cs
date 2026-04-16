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
                Codigo       = Convert.ToInt32(r["Codigo"]),
                auxCodigo    = Convert.ToInt32(r["auxCodigo"]),
                marDescricao = r["marDescricao"]?.ToString() ?? "",
                marValor     = Convert.ToDecimal(r["marValor"]),
                marCusto     = r["marCusto"] == DBNull.Value ? 0m : Convert.ToDecimal(r["marCusto"]),
                Situacao     = (r["Situacao"]?.ToString() ?? "A")[0]
            };
        }

        public string Incluir(Marmita obj)
        {
            try
            {
                using var conn = AbrirConexao();
                obj.Codigo    = ProximoCodigo("marmita", conn);
                obj.auxCodigo = ProximoAuxCodigo("marmita", conn);
                using var cmd = new MySqlCommand(@"
                    INSERT INTO marmita (Codigo, auxCodigo, marDescricao, marValor, marCusto, Situacao)
                    VALUES (@cod, @aux, @desc, @val, @cst, @sit)", conn);
                cmd.Parameters.AddWithValue("@cod",  obj.Codigo);
                cmd.Parameters.AddWithValue("@aux",  obj.auxCodigo);
                cmd.Parameters.AddWithValue("@desc", obj.marDescricao);
                cmd.Parameters.AddWithValue("@val",  obj.marValor);
                cmd.Parameters.AddWithValue("@cst",  obj.marCusto);
                cmd.Parameters.AddWithValue("@sit",  obj.Situacao.ToString());
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
                    UPDATE marmita SET marDescricao=@desc, marValor=@val, marCusto=@cst, Situacao=@sit
                    WHERE Codigo=@cod", conn);
                cmd.Parameters.AddWithValue("@desc", obj.marDescricao);
                cmd.Parameters.AddWithValue("@val",  obj.marValor);
                cmd.Parameters.AddWithValue("@cst",  obj.marCusto);
                cmd.Parameters.AddWithValue("@sit",  obj.Situacao.ToString());
                cmd.Parameters.AddWithValue("@cod",  obj.Codigo);
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
                    maritmQtde        = Convert.ToDecimal(r["maritmQtde"])
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
                    INSERT INTO marmita_item (Codigo, auxCodigo, Codigo_Marmita, maritmCodigo_Merc, maritmNome, maritmQtde)
                    VALUES (@cod, @aux, @mar, @merc, @nome, @qtde)", conn);
                cmd.Parameters.AddWithValue("@cod",  item.Codigo);
                cmd.Parameters.AddWithValue("@aux",  item.auxCodigo);
                cmd.Parameters.AddWithValue("@mar",  item.Codigo_Marmita);
                cmd.Parameters.AddWithValue("@merc", item.maritmCodigo_Merc);
                cmd.Parameters.AddWithValue("@nome", item.maritmNome);
                cmd.Parameters.AddWithValue("@qtde", item.maritmQtde);
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
    }
}

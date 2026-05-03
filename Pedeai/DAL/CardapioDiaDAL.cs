using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;

namespace Pedeai.DAL
{
    public class CardapioDiaDAL : BaseDAL
    {
        /// <summary>Retorna o cardápio da data informada, ou null se não existir.</summary>
        public (int codigo, string titulo, string observacao) BuscarPorData(DateTime data)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT Codigo, card_Titulo, card_Observacao FROM cardapio_dia WHERE card_Data=@dt LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@dt", data.Date);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return (0, "", "");
            return (Convert.ToInt32(r["Codigo"]), r["card_Titulo"]?.ToString() ?? "", r["card_Observacao"]?.ToString() ?? "");
        }

        public DataTable Listar()
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT Codigo, card_Data AS Data, card_Titulo AS Titulo FROM cardapio_dia ORDER BY card_Data DESC", conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public (int codigo, string titulo, string observacao) BuscarPorCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT Codigo, card_Titulo, card_Observacao FROM cardapio_dia WHERE Codigo=@id LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@id", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return (0, "", "");
            return (Convert.ToInt32(r["Codigo"]), r["card_Titulo"]?.ToString() ?? "", r["card_Observacao"]?.ToString() ?? "");
        }

        public int SalvarCabecalho(int codigo, DateTime data, string titulo, string observacao)
        {
            using var conn = AbrirConexao();
            if (codigo > 0)
            {
                using var upd = new MySqlCommand(
                    "UPDATE cardapio_dia SET card_Data=@dt, card_Titulo=@tit, card_Observacao=@obs WHERE Codigo=@id", conn);
                upd.Parameters.AddWithValue("@dt",  data.Date);
                upd.Parameters.AddWithValue("@tit", titulo ?? "");
                upd.Parameters.AddWithValue("@obs", observacao ?? "");
                upd.Parameters.AddWithValue("@id",  codigo);
                upd.ExecuteNonQuery();
                return codigo;
            }
            using var cmd = new MySqlCommand(@"
                INSERT INTO cardapio_dia (card_Data, card_Titulo, card_Observacao)
                VALUES (@dt, @tit, @obs)", conn);
            cmd.Parameters.AddWithValue("@dt",  data.Date);
            cmd.Parameters.AddWithValue("@tit", titulo ?? "");
            cmd.Parameters.AddWithValue("@obs", observacao ?? "");
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public void AtualizarCabecalho(int codigo, string titulo, string observacao)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "UPDATE cardapio_dia SET card_Titulo=@tit, card_Observacao=@obs WHERE Codigo=@id", conn);
            cmd.Parameters.AddWithValue("@tit", titulo ?? "");
            cmd.Parameters.AddWithValue("@obs", observacao ?? "");
            cmd.Parameters.AddWithValue("@id",  codigo);
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int codigo)
        {
            using var conn = AbrirConexao();
            using var c1 = new MySqlCommand("DELETE FROM cardapio_dia_item WHERE Codigo_Cardapio=@id", conn);
            c1.Parameters.AddWithValue("@id", codigo); c1.ExecuteNonQuery();
            using var c2 = new MySqlCommand("DELETE FROM cardapio_dia WHERE Codigo=@id", conn);
            c2.Parameters.AddWithValue("@id", codigo); c2.ExecuteNonQuery();
        }

        public List<(int cod, int codigoMerc, string nome, string descr)> ListarItens(int codigoCardapio)
        {
            var lista = new List<(int, int, string, string)>();
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(@"
                SELECT Codigo, Codigo_Mercadoria, card_Produto_Nome, card_Produto_Descricao
                FROM cardapio_dia_item WHERE Codigo_Cardapio=@id ORDER BY Codigo", conn);
            cmd.Parameters.AddWithValue("@id", codigoCardapio);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add((
                    Convert.ToInt32(r["Codigo"]),
                    r["Codigo_Mercadoria"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Mercadoria"]),
                    r["card_Produto_Nome"]?.ToString() ?? "",
                    r["card_Produto_Descricao"]?.ToString() ?? ""
                ));
            return lista;
        }

        public void AdicionarItem(int codigoCardapio, int codigoMerc, string nome, string descr)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(@"
                INSERT INTO cardapio_dia_item
                    (Codigo_Cardapio, Codigo_Mercadoria, card_Produto_Nome, card_Produto_Descricao)
                VALUES (@card, @merc, @nome, @desc)", conn);
            cmd.Parameters.AddWithValue("@card", codigoCardapio);
            cmd.Parameters.AddWithValue("@merc", codigoMerc);
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@desc", descr ?? "");
            cmd.ExecuteNonQuery();
        }

        public void RemoverItem(int codigoItem)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand("DELETE FROM cardapio_dia_item WHERE Codigo=@id", conn);
            cmd.Parameters.AddWithValue("@id", codigoItem);
            cmd.ExecuteNonQuery();
        }

        public void LimparItens(int codigoCardapio)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand("DELETE FROM cardapio_dia_item WHERE Codigo_Cardapio=@id", conn);
            cmd.Parameters.AddWithValue("@id", codigoCardapio);
            cmd.ExecuteNonQuery();
        }
    }
}

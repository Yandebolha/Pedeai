using System;
using System.Collections.Generic;
using System.Data;
using MySqlConnector;

namespace Pedeai.DAL
{
    public class PromocaoDAL : BaseDAL
    {
        public DataTable Listar()
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(@"
                SELECT p.Codigo, p.prom_Nome AS Nome,
                       p.prom_DataInicio AS Inicio, p.prom_DataFim AS Fim,
                       p.prom_Desconto_Tipo AS TipoDesc,
                       p.prom_Desconto_Valor AS ValorDesc,
                       p.prom_Ativo AS Ativo,
                       COUNT(i.Codigo) AS Produtos
                FROM promocao p
                LEFT JOIN promocao_item i ON i.Codigo_Promocao = p.Codigo
                GROUP BY p.Codigo
                ORDER BY p.prom_DataFim DESC", conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public int Salvar(string nome, DateTime inicio, DateTime fim,
            string tipoDesc, decimal valorDesc)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(@"
                INSERT INTO promocao (prom_Nome, prom_DataInicio, prom_DataFim,
                    prom_Desconto_Tipo, prom_Desconto_Valor, prom_Ativo)
                VALUES (@nome, @ini, @fim, @tipo, @val, 1)", conn);
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@ini",  inicio.Date);
            cmd.Parameters.AddWithValue("@fim",  fim.Date.Add(new TimeSpan(23, 59, 59)));
            cmd.Parameters.AddWithValue("@tipo", tipoDesc);
            cmd.Parameters.AddWithValue("@val",  valorDesc);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public void Atualizar(int codigo, string nome, DateTime inicio, DateTime fim,
            string tipoDesc, decimal valorDesc)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(@"
                UPDATE promocao SET prom_Nome=@nome, prom_DataInicio=@ini, prom_DataFim=@fim,
                    prom_Desconto_Tipo=@tipo, prom_Desconto_Valor=@val
                WHERE Codigo=@id", conn);
            cmd.Parameters.AddWithValue("@nome", nome);
            cmd.Parameters.AddWithValue("@ini",  inicio.Date);
            cmd.Parameters.AddWithValue("@fim",  fim.Date.Add(new TimeSpan(23, 59, 59)));
            cmd.Parameters.AddWithValue("@tipo", tipoDesc);
            cmd.Parameters.AddWithValue("@val",  valorDesc);
            cmd.Parameters.AddWithValue("@id",   codigo);
            cmd.ExecuteNonQuery();
        }

        public void Excluir(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand("DELETE FROM promocao_item WHERE Codigo_Promocao=@id", conn);
            cmd.Parameters.AddWithValue("@id", codigo);
            cmd.ExecuteNonQuery();
            using var cmd2 = new MySqlCommand("DELETE FROM promocao WHERE Codigo=@id", conn);
            cmd2.Parameters.AddWithValue("@id", codigo);
            cmd2.ExecuteNonQuery();
        }

        public List<(int cod, string nome, decimal preco)> ListarItens(int codigoPromocao)
        {
            var lista = new List<(int, string, decimal)>();
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(@"
                SELECT i.Codigo, i.prom_Produto_Nome,
                       COALESCE(m.mercPreco_Venda, 0) AS Preco
                FROM promocao_item i
                LEFT JOIN mercadoria m ON m.Codigo = i.Codigo_Mercadoria
                WHERE i.Codigo_Promocao = @id
                ORDER BY i.Codigo", conn);
            cmd.Parameters.AddWithValue("@id", codigoPromocao);
            using var r = cmd.ExecuteReader();
            while (r.Read())
                lista.Add((
                    Convert.ToInt32(r["Codigo"]),
                    r["prom_Produto_Nome"]?.ToString() ?? "",
                    r["Preco"] == DBNull.Value ? 0m : Convert.ToDecimal(r["Preco"])
                ));
            return lista;
        }

        public void AdicionarItem(int codigoPromocao, int codigoMercadoria, string nomeProduto)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(@"
                INSERT INTO promocao_item (Codigo_Promocao, Codigo_Mercadoria, prom_Produto_Nome)
                VALUES (@prom, @merc, @nome)", conn);
            cmd.Parameters.AddWithValue("@prom", codigoPromocao);
            cmd.Parameters.AddWithValue("@merc", codigoMercadoria);
            cmd.Parameters.AddWithValue("@nome", nomeProduto);
            cmd.ExecuteNonQuery();
        }

        public void RemoverItem(int codigoItem)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand("DELETE FROM promocao_item WHERE Codigo=@id", conn);
            cmd.Parameters.AddWithValue("@id", codigoItem);
            cmd.ExecuteNonQuery();
        }
    }
}

using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class MercadoriaDAL : BaseDAL
    {
        /// <summary>Lista mercadorias com join na categoria.</summary>
        public DataTable Listar()
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT m.Codigo,
                               m.mercMercadoria        AS Nome,
                               g.grmeDescricao_        AS Categoria,
                               m.mercPreco_Venda        AS Preco,
                               m.mercPreco_Promocional  AS Promocional,
                               m.mercEstoque_Atual      AS Estoque,
                               m.Situacao
                        FROM mercadoria m
                        LEFT JOIN grupo_mercadoria g ON g.Codigo = m.Codigo_Grupo
                        ORDER BY g.grmeOrdem, m.mercOrdem, m.mercMercadoria";
            using var cmd = new MySqlCommand(sql, conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        /// <summary>Busca mercadoria completa pelo Codigo.</summary>
        public Mercadoria PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM mercadoria WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapearMercadoria(r);
        }

        /// <summary>Inclui nova mercadoria. Gera Codigo/auxCodigo automaticamente.</summary>
        public string Incluir(Mercadoria obj)
        {
            try
            {
                using var conn = AbrirConexao();
                obj.Codigo    = ProximoCodigo("mercadoria", conn);
                obj.auxCodigo = ProximoAuxCodigo("mercadoria", conn);
                obj.mercData_Cadastro = DateTime.Now;

                var sql = @"INSERT INTO mercadoria
                            (auxCodigo, Codigo, Codigo_Grupo, mercMercadoria, mercApresentacao,
                             mercPreco_Venda, mercPreco_Custo, mercPreco_Promocional, mercEstoque_Atual,
                             mercControla_Estoque, mercImagem_Url, mercDestaque, mercOrdem,
                             mercHabilitar_Ifood, Situacao, Status_Transmissao, Info, mercData_Cadastro)
                            VALUES(@aux, @cod, @grp, @nome, @desc, @preco, @custo, @promo, @est,
                                   @ctrl, @img, @dest, @ordem, @ifood, @sit, @trans, @info, @dt)";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Altera mercadoria existente.</summary>
        public string Alterar(Mercadoria obj)
        {
            try
            {
                using var conn = AbrirConexao();
                var sql = @"UPDATE mercadoria SET
                            Codigo_Grupo=@grp, mercMercadoria=@nome, mercApresentacao=@desc,
                            mercPreco_Venda=@preco, mercPreco_Custo=@custo, mercPreco_Promocional=@promo, mercEstoque_Atual=@est,
                            mercControla_Estoque=@ctrl, mercImagem_Url=@img, mercDestaque=@dest,
                            mercOrdem=@ordem, mercHabilitar_Ifood=@ifood, Situacao=@sit
                            WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        /// <summary>Alterna Situacao entre 'A' (Ativo) e 'I' (Inativo).</summary>
        public void AlternarSituacao(int codigo)
        {
            using var conn = AbrirConexao();
            var sql = "UPDATE mercadoria SET Situacao = IF(Situacao='A','I','A') WHERE Codigo = @cod";
            using var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            cmd.ExecuteNonQuery();
        }

        private static void BindParams(MySqlCommand cmd, Mercadoria obj)
        {
            cmd.Parameters.AddWithValue("@aux",   obj.auxCodigo);
            cmd.Parameters.AddWithValue("@cod",   obj.Codigo);
            cmd.Parameters.AddWithValue("@grp",   obj.Codigo_Grupo);
            cmd.Parameters.AddWithValue("@nome",  obj.mercMercadoria);
            cmd.Parameters.AddWithValue("@desc",  obj.mercApresentacao ?? "");
            cmd.Parameters.AddWithValue("@preco", obj.mercPreco_Venda);
            cmd.Parameters.AddWithValue("@custo", obj.mercPreco_Custo);
            cmd.Parameters.AddWithValue("@promo", obj.mercPreco_Promocional);
            cmd.Parameters.AddWithValue("@est",   obj.mercEstoque_Atual);
            cmd.Parameters.AddWithValue("@ctrl",  obj.mercControla_Estoque ? 1 : 0);
            cmd.Parameters.AddWithValue("@img",   obj.mercImagem_Url ?? "");
            cmd.Parameters.AddWithValue("@dest",  obj.mercDestaque ? 1 : 0);
            cmd.Parameters.AddWithValue("@ordem", obj.mercOrdem);
            cmd.Parameters.AddWithValue("@ifood", obj.mercHabilitar_Ifood ? 1 : 0);
            cmd.Parameters.AddWithValue("@sit",   obj.Situacao ?? "A");
            cmd.Parameters.AddWithValue("@trans", obj.Status_Transmissao ?? "N");
            cmd.Parameters.AddWithValue("@info",  obj.Info ?? "");
            cmd.Parameters.AddWithValue("@dt",    obj.mercData_Cadastro);
        }

        private static Mercadoria MapearMercadoria(MySqlDataReader r)
        {
            return new Mercadoria
            {
                Codigo               = Convert.ToInt32(r["Codigo"]),
                auxCodigo            = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
                Codigo_Grupo         = r["Codigo_Grupo"] == DBNull.Value ? 0 : Convert.ToInt32(r["Codigo_Grupo"]),
                mercMercadoria       = r["mercMercadoria"]?.ToString() ?? "",
                mercApresentacao     = r["mercApresentacao"]?.ToString() ?? "",
                mercPreco_Venda      = r["mercPreco_Venda"] == DBNull.Value ? 0 : Convert.ToDecimal(r["mercPreco_Venda"]),
                mercPreco_Custo      = r["mercPreco_Custo"] == DBNull.Value ? 0 : Convert.ToDecimal(r["mercPreco_Custo"]),
                mercPreco_Promocional= r["mercPreco_Promocional"] == DBNull.Value ? 0 : Convert.ToDecimal(r["mercPreco_Promocional"]),
                mercEstoque_Atual    = r["mercEstoque_Atual"] == DBNull.Value ? 0 : Convert.ToDecimal(r["mercEstoque_Atual"]),
                mercControla_Estoque = r["mercControla_Estoque"]?.ToString() == "1" || r["mercControla_Estoque"]?.ToString() == "True",
                mercImagem_Url       = r["mercImagem_Url"]?.ToString() ?? "",
                mercDestaque         = r["mercDestaque"]?.ToString() == "1" || r["mercDestaque"]?.ToString() == "True",
                mercOrdem            = r["mercOrdem"] == DBNull.Value ? 0 : Convert.ToInt32(r["mercOrdem"]),
                mercHabilitar_Ifood  = r["mercHabilitar_Ifood"]?.ToString() == "1" || r["mercHabilitar_Ifood"]?.ToString() == "True",
                mercData_Cadastro    = r["mercData_Cadastro"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["mercData_Cadastro"]),
                Situacao             = r["Situacao"]?.ToString() ?? "A",
                Status_Transmissao   = r["Status_Transmissao"]?.ToString() ?? "N",
                Info                 = r["Info"]?.ToString() ?? "",
            };
        }
    }
}

using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class CupomDAL : BaseDAL
    {
        public DataTable Listar()
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT Codigo,
                               cupomCodigo        AS Cupom,
                               cupomDescricao     AS Descricao,
                               cupomTipo          AS Tipo,
                               cupomValor         AS Valor,
                               cupomUsos_Realizados AS Usos,
                               cupomValido_Ate    AS ValidoAte,
                               Situacao
                        FROM cupom
                        WHERE cupomCodigo NOT LIKE 'FID%'
                        ORDER BY cupomData_Cadastro DESC";
            using var cmd = new MySqlCommand(sql, conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public Cupom PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM cupom WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapearCupom(r);
        }

        public Cupom BuscarPorTexto(string cupomCodigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM cupom WHERE cupomCodigo = @c AND Situacao = 'A' LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@c", cupomCodigo?.Trim() ?? "");
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapearCupom(r);
        }

        public string Incluir(Cupom obj)
        {
            try
            {
                using var conn = AbrirConexao();

                // Valida unicidade do código do cupom
                using (var chk = new MySqlCommand(
                    "SELECT COUNT(*) FROM cupom WHERE cupomCodigo = @c", conn))
                {
                    chk.Parameters.AddWithValue("@c", obj.cupomCodigo);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return "Já existe um cupom com esse código.";
                }

                obj.Codigo    = ProximoCodigo("cupom", conn);
                obj.auxCodigo = ProximoAuxCodigo("cupom", conn);
                obj.cupomData_Cadastro = DateTime.Now;

                var sql = @"INSERT INTO cupom
                            (auxCodigo, Codigo, cupomCodigo, cupomDescricao, cupomTipo, cupomValor,
                             cupomPedido_Minimo, cupomLimite_Usos, cupomUsos_Realizados,
                             cupomValido_Ate, cupomData_Cadastro, Situacao, Status_Transmissao, Info)
                            VALUES(@aux, @cod, @cupom, @desc, @tipo, @valor, @minimo, @limite,
                                   0, @valido, @dt, @sit, @trans, @info)";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(Cupom obj)
        {
            try
            {
                using var conn = AbrirConexao();
                var sql = @"UPDATE cupom SET
                            cupomCodigo=@cupom, cupomDescricao=@desc, cupomTipo=@tipo, cupomValor=@valor,
                            cupomPedido_Minimo=@minimo, cupomLimite_Usos=@limite,
                            cupomValido_Ate=@valido, Situacao=@sit
                            WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private static void BindParams(MySqlCommand cmd, Cupom obj)
        {
            cmd.Parameters.AddWithValue("@aux",    obj.auxCodigo);
            cmd.Parameters.AddWithValue("@cod",    obj.Codigo);
            cmd.Parameters.AddWithValue("@cupom",  obj.cupomCodigo ?? "");
            cmd.Parameters.AddWithValue("@desc",   obj.cupomDescricao ?? "");
            cmd.Parameters.AddWithValue("@tipo",   obj.cupomTipo ?? "PERCENTUAL");
            cmd.Parameters.AddWithValue("@valor",  obj.cupomValor);
            cmd.Parameters.AddWithValue("@minimo", obj.cupomPedido_Minimo);
            cmd.Parameters.AddWithValue("@limite", obj.cupomLimite_Usos);
            cmd.Parameters.AddWithValue("@valido", obj.cupomValido_Ate.Date);
            cmd.Parameters.AddWithValue("@sit",    obj.Situacao ?? "A");
            cmd.Parameters.AddWithValue("@trans",  obj.Status_Transmissao ?? "N");
            cmd.Parameters.AddWithValue("@info",   obj.Info ?? "");
            cmd.Parameters.AddWithValue("@dt",     obj.cupomData_Cadastro);
        }

        private static Cupom MapearCupom(MySqlDataReader r)
        {
            return new Cupom
            {
                Codigo               = Convert.ToInt32(r["Codigo"]),
                auxCodigo            = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
                cupomCodigo          = r["cupomCodigo"]?.ToString() ?? "",
                cupomDescricao       = r["cupomDescricao"]?.ToString() ?? "",
                cupomTipo            = r["cupomTipo"]?.ToString() ?? "PERCENTUAL",
                cupomValor           = r["cupomValor"] == DBNull.Value ? 0 : Convert.ToDecimal(r["cupomValor"]),
                cupomPedido_Minimo   = r["cupomPedido_Minimo"] == DBNull.Value ? 0 : Convert.ToDecimal(r["cupomPedido_Minimo"]),
                cupomLimite_Usos     = r["cupomLimite_Usos"] == DBNull.Value ? 0 : Convert.ToInt32(r["cupomLimite_Usos"]),
                cupomUsos_Realizados = r["cupomUsos_Realizados"] == DBNull.Value ? 0 : Convert.ToInt32(r["cupomUsos_Realizados"]),
                cupomValido_Ate      = r["cupomValido_Ate"] == DBNull.Value ? DateTime.Today : Convert.ToDateTime(r["cupomValido_Ate"]),
                cupomData_Cadastro   = r["cupomData_Cadastro"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["cupomData_Cadastro"]),
                Situacao             = r["Situacao"]?.ToString() ?? "A",
                Status_Transmissao   = r["Status_Transmissao"]?.ToString() ?? "N",
                Info                 = r["Info"]?.ToString() ?? "",
            };
        }
    }
}

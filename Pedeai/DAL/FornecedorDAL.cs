using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class FornecedorDAL : BaseDAL
    {
        public DataTable Listar()
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT Codigo,
                               fornNome_RazaoSocial  AS RazaoSocial,
                               fornApelido_Fantasia  AS NomeFantasia,
                               fornCPF_CNPJ_         AS CNPJ,
                               fornTelefone          AS Telefone,
                               fornEmail             AS Email,
                               fornContato           AS Contato,
                               Situacao
                        FROM fornecedor ORDER BY fornNome_RazaoSocial";
            using var cmd = new MySqlCommand(sql, conn);
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public Fornecedor PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM fornecedor WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapearFornecedor(r);
        }

        public string Incluir(Fornecedor obj)
        {
            try
            {
                using var conn = AbrirConexao();

                if (!string.IsNullOrWhiteSpace(obj.fornCPF_CNPJ_))
                {
                    using var chk = new MySqlCommand(
                        "SELECT COUNT(*) FROM fornecedor WHERE fornCPF_CNPJ_ = @cnpj AND Situacao = 'A'", conn);
                    chk.Parameters.AddWithValue("@cnpj", obj.fornCPF_CNPJ_.Trim());
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return $"J\u00e1 existe um fornecedor cadastrado com o CNPJ/CPF '{obj.fornCPF_CNPJ_}'.";
                }

                obj.Codigo    = ProximoCodigo("fornecedor", conn);
                obj.auxCodigo = ProximoAuxCodigo("fornecedor", conn);
                obj.fornData_Cadastro = DateTime.Now;

                var sql = @"INSERT INTO fornecedor
                            (auxCodigo, Codigo, fornNome_RazaoSocial, fornApelido_Fantasia, fornCPF_CNPJ_,
                             fornRG_InscricaoEstadual, fornTelefone, fornEmail, fornContato, fornCEP,
                             fornEndereco, fornNumero, fornBairro, fornCidade, fornEstado,
                             fornObservacoes, fornData_Cadastro, Situacao, Status_Transmissao, Info)
                            VALUES(@aux, @cod, @razao, @fantasia, @cnpj, @ie, @tel, @email, @cont, @cep,
                                   @end, @num, @bairro, @cidade, @estado, @obs, @dt, @sit, @trans, @info)";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(Fornecedor obj)
        {
            try
            {
                using var conn = AbrirConexao();

                if (!string.IsNullOrWhiteSpace(obj.fornCPF_CNPJ_))
                {
                    using var chk = new MySqlCommand(
                        "SELECT COUNT(*) FROM fornecedor WHERE fornCPF_CNPJ_ = @cnpj AND Situacao = 'A' AND Codigo <> @cod", conn);
                    chk.Parameters.AddWithValue("@cnpj", obj.fornCPF_CNPJ_.Trim());
                    chk.Parameters.AddWithValue("@cod",  obj.Codigo);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return $"J\u00e1 existe outro fornecedor com o CNPJ/CPF '{obj.fornCPF_CNPJ_}'.";
                }

                var sql = @"UPDATE fornecedor SET
                            fornNome_RazaoSocial=@razao, fornApelido_Fantasia=@fantasia, fornCPF_CNPJ_=@cnpj,
                            fornRG_InscricaoEstadual=@ie, fornTelefone=@tel, fornEmail=@email, fornContato=@cont,
                            fornCEP=@cep, fornEndereco=@end, fornNumero=@num, fornBairro=@bairro,
                            fornCidade=@cidade, fornEstado=@estado, fornObservacoes=@obs, Situacao=@sit
                            WHERE Codigo=@cod";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private static void BindParams(MySqlCommand cmd, Fornecedor obj)
        {
            cmd.Parameters.AddWithValue("@aux",     obj.auxCodigo);
            cmd.Parameters.AddWithValue("@cod",     obj.Codigo);
            cmd.Parameters.AddWithValue("@razao",   obj.fornNome_RazaoSocial ?? "");
            cmd.Parameters.AddWithValue("@fantasia",obj.fornApelido_Fantasia ?? "");
            cmd.Parameters.AddWithValue("@cnpj",    obj.fornCPF_CNPJ_ ?? "");
            cmd.Parameters.AddWithValue("@ie",      obj.fornRG_InscricaoEstadual ?? "");
            cmd.Parameters.AddWithValue("@tel",     obj.fornTelefone ?? "");
            cmd.Parameters.AddWithValue("@email",   obj.fornEmail ?? "");
            cmd.Parameters.AddWithValue("@cont",    obj.fornContato ?? "");
            cmd.Parameters.AddWithValue("@cep",     obj.fornCEP ?? "");
            cmd.Parameters.AddWithValue("@end",     obj.fornEndereco ?? "");
            cmd.Parameters.AddWithValue("@num",     obj.fornNumero ?? "");
            cmd.Parameters.AddWithValue("@bairro",  obj.fornBairro ?? "");
            cmd.Parameters.AddWithValue("@cidade",  obj.fornCidade ?? "");
            cmd.Parameters.AddWithValue("@estado",  obj.fornEstado ?? "");
            cmd.Parameters.AddWithValue("@obs",     obj.fornObservacoes ?? "");
            cmd.Parameters.AddWithValue("@sit",     obj.Situacao ?? "A");
            cmd.Parameters.AddWithValue("@trans",   obj.Status_Transmissao ?? "N");
            cmd.Parameters.AddWithValue("@info",    obj.Info ?? "");
            cmd.Parameters.AddWithValue("@dt",      obj.fornData_Cadastro);
        }

        private static Fornecedor MapearFornecedor(MySqlDataReader r)
        {
            return new Fornecedor
            {
                Codigo                   = Convert.ToInt32(r["Codigo"]),
                auxCodigo                = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
                fornNome_RazaoSocial     = r["fornNome_RazaoSocial"]?.ToString() ?? "",
                fornApelido_Fantasia     = r["fornApelido_Fantasia"]?.ToString() ?? "",
                fornCPF_CNPJ_            = r["fornCPF_CNPJ_"]?.ToString() ?? "",
                fornRG_InscricaoEstadual = r["fornRG_InscricaoEstadual"]?.ToString() ?? "",
                fornTelefone             = r["fornTelefone"]?.ToString() ?? "",
                fornEmail                = r["fornEmail"]?.ToString() ?? "",
                fornContato              = r["fornContato"]?.ToString() ?? "",
                fornCEP                  = r["fornCEP"]?.ToString() ?? "",
                fornEndereco             = r["fornEndereco"]?.ToString() ?? "",
                fornNumero               = r["fornNumero"]?.ToString() ?? "",
                fornBairro               = r["fornBairro"]?.ToString() ?? "",
                fornCidade               = r["fornCidade"]?.ToString() ?? "",
                fornEstado               = r["fornEstado"]?.ToString() ?? "",
                fornObservacoes          = r["fornObservacoes"]?.ToString() ?? "",
                fornData_Cadastro        = r["fornData_Cadastro"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["fornData_Cadastro"]),
                Situacao                 = r["Situacao"]?.ToString() ?? "A",
                Status_Transmissao       = r["Status_Transmissao"]?.ToString() ?? "N",
                Info                     = r["Info"]?.ToString() ?? "",
            };
        }
    }
}

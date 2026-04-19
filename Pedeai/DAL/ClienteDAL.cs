using System;
using System.Data;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class ClienteDAL : BaseDAL
    {
        public DataTable Listar(string busca = "")
        {
            var dt = new DataTable();
            using var conn = AbrirConexao();
            var sql = @"SELECT c.Codigo,
                               c.clieNome_RazaoSocial AS Nome,
                               c.clieCelular          AS Celular,
                               c.clieEmail            AS Email,
                               CONCAT(c.clieEndereco,
                                      IF(c.clieNumero <> '', CONCAT(', ', c.clieNumero), '')) AS Endereco,
                               c.clieCidade           AS Cidade,
                               COALESCE((SELECT COUNT(*) FROM pedido_web pw
                                         WHERE pw.Codigo_Cliente = c.Codigo
                                           AND pw.pediSituacao <> 6), 0) AS TotalPedidos,
                               COALESCE((SELECT SUM(pw.pediValor_Total) FROM pedido_web pw
                                         WHERE pw.Codigo_Cliente = c.Codigo
                                           AND pw.pediSituacao <> 6), 0) AS TotalGasto,
                               COALESCE(c.clieGasto_Mensal, 0) AS GastoMensal,
                               COALESCE(c.cliePedidos_Mensal, 0) AS PedidosMensal,
                               c.Situacao
                        FROM cliente c WHERE 1=1";
            if (!string.IsNullOrWhiteSpace(busca))
                sql += " AND (c.clieNome_RazaoSocial LIKE @b OR c.clieTelefone LIKE @b OR c.clieCelular LIKE @b)";
            sql += " ORDER BY c.clieNome_RazaoSocial ASC LIMIT 200";

            using var cmd = new MySqlCommand(sql, conn);
            if (!string.IsNullOrWhiteSpace(busca)) cmd.Parameters.AddWithValue("@b", $"%{busca}%");
            new MySqlDataAdapter(cmd).Fill(dt);
            return dt;
        }

        public Cliente PesquisaCodigo(int codigo)
        {
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                "SELECT * FROM cliente WHERE Codigo = @cod LIMIT 1", conn);
            cmd.Parameters.AddWithValue("@cod", codigo);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return null;
            return MapearCliente(r);
        }

        public string Incluir(Cliente obj)
        {
            try
            {
                using var conn = AbrirConexao();

                if (!string.IsNullOrWhiteSpace(obj.clieCPF_CNPJ_))
                {
                    using var chk = new MySqlCommand(
                        "SELECT COUNT(*) FROM cliente WHERE clieCPF_CNPJ_ = @cpf AND Situacao = 'A'", conn);
                    chk.Parameters.AddWithValue("@cpf", obj.clieCPF_CNPJ_.Trim());
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return $"J\u00e1 existe um cliente cadastrado com o CPF/CNPJ '{obj.clieCPF_CNPJ_}'.";
                }

                obj.Codigo    = ProximoCodigo("cliente", conn);
                obj.auxCodigo = ProximoAuxCodigo("cliente", conn);
                obj.clieData_Cadastro = DateTime.Now;

                var sql = @"INSERT INTO cliente
                            (auxCodigo, Codigo, clieNome_RazaoSocial, clieTelefone, clieCelular,
                             clieEmail, clieCPF_CNPJ_, clieCEP, clieEndereco, clieNumero,
                             clieComplemento, clieBairro, clieCidade, clieEstado,
                             clieTotalPedidos, clieTotalGasto, clieData_Cadastro, Situacao, Status_Transmissao, Info)
                            VALUES(@aux, @cod, @nome, @tel, @cel, @email, @cpf, @cep, @end, @num,
                                   @comp, @bairro, @cidade, @estado, 0, 0, @dt, @sit, @trans, @info)";
                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        public string Alterar(Cliente obj)
        {
            try
            {
                using var conn = AbrirConexao();

                if (!string.IsNullOrWhiteSpace(obj.clieCPF_CNPJ_))
                {
                    using var chk = new MySqlCommand(
                        "SELECT COUNT(*) FROM cliente WHERE clieCPF_CNPJ_ = @cpf AND Situacao = 'A' AND Codigo <> @cod", conn);
                    chk.Parameters.AddWithValue("@cpf", obj.clieCPF_CNPJ_.Trim());
                    chk.Parameters.AddWithValue("@cod", obj.Codigo);
                    if (Convert.ToInt32(chk.ExecuteScalar()) > 0)
                        return $"J\u00e1 existe outro cliente com o CPF/CNPJ '{obj.clieCPF_CNPJ_}'.";
                }
                var sql = @"UPDATE cliente SET
                            clieNome_RazaoSocial=@nome, clieTelefone=@tel, clieCelular=@cel,
                            clieEmail=@email, clieCPF_CNPJ_=@cpf, clieCEP=@cep,
                            clieEndereco=@end, clieNumero=@num, clieComplemento=@comp,
                            clieBairro=@bairro, clieCidade=@cidade, clieEstado=@estado, Situacao=@sit
                            WHERE Codigo=@cod";                using var cmd = new MySqlCommand(sql, conn);
                BindParams(cmd, obj);
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }

        private static void BindParams(MySqlCommand cmd, Cliente obj)
        {
            cmd.Parameters.AddWithValue("@aux",    obj.auxCodigo);
            cmd.Parameters.AddWithValue("@cod",    obj.Codigo);
            cmd.Parameters.AddWithValue("@nome",   obj.clieNome_RazaoSocial ?? "");
            cmd.Parameters.AddWithValue("@tel",    obj.clieTelefone ?? "");
            cmd.Parameters.AddWithValue("@cel",    obj.clieCelular ?? "");
            cmd.Parameters.AddWithValue("@email",  obj.clieEmail ?? "");
            cmd.Parameters.AddWithValue("@cpf",    obj.clieCPF_CNPJ_ ?? "");
            cmd.Parameters.AddWithValue("@cep",    obj.clieCEP ?? "");
            cmd.Parameters.AddWithValue("@end",    obj.clieEndereco ?? "");
            cmd.Parameters.AddWithValue("@num",    obj.clieNumero ?? "");
            cmd.Parameters.AddWithValue("@comp",   obj.clieComplemento ?? "");
            cmd.Parameters.AddWithValue("@bairro", obj.clieBairro ?? "");
            cmd.Parameters.AddWithValue("@cidade", obj.clieCidade ?? "");
            cmd.Parameters.AddWithValue("@estado", obj.clieEstado ?? "");
            cmd.Parameters.AddWithValue("@sit",    obj.Situacao ?? "A");
            cmd.Parameters.AddWithValue("@trans",  obj.Status_Transmissao ?? "N");
            cmd.Parameters.AddWithValue("@info",   obj.Info ?? "");
            cmd.Parameters.AddWithValue("@dt",     obj.clieData_Cadastro);
        }

        private static Cliente MapearCliente(MySqlDataReader r)
        {
            return new Cliente
            {
                Codigo               = Convert.ToInt32(r["Codigo"]),
                auxCodigo            = r["auxCodigo"] == DBNull.Value ? 0 : Convert.ToInt32(r["auxCodigo"]),
                clieNome_RazaoSocial = r["clieNome_RazaoSocial"]?.ToString() ?? "",
                clieTelefone         = r["clieTelefone"]?.ToString() ?? "",
                clieCelular          = r["clieCelular"]?.ToString() ?? "",
                clieEmail            = r["clieEmail"]?.ToString() ?? "",
                clieCPF_CNPJ_        = r["clieCPF_CNPJ_"]?.ToString() ?? "",
                clieCEP              = r["clieCEP"]?.ToString() ?? "",
                clieEndereco         = r["clieEndereco"]?.ToString() ?? "",
                clieNumero           = r["clieNumero"]?.ToString() ?? "",
                clieComplemento      = r["clieComplemento"]?.ToString() ?? "",
                clieBairro           = r["clieBairro"]?.ToString() ?? "",
                clieCidade           = r["clieCidade"]?.ToString() ?? "",
                clieEstado           = r["clieEstado"]?.ToString() ?? "",
                clieTotalPedidos     = r["clieTotalPedidos"] == DBNull.Value ? 0 : Convert.ToInt32(r["clieTotalPedidos"]),
                clieTotalGasto       = r["clieTotalGasto"] == DBNull.Value ? 0 : Convert.ToDecimal(r["clieTotalGasto"]),
                clieGasto_Mensal     = r["clieGasto_Mensal"] == DBNull.Value ? 0 : Convert.ToDecimal(r["clieGasto_Mensal"]),
                clieGasto_Mes_Ref    = r["clieGasto_Mes_Ref"] == DBNull.Value ? "" : r["clieGasto_Mes_Ref"].ToString(),
                cliePedidos_Mensal   = r["cliePedidos_Mensal"] == DBNull.Value ? 0 : Convert.ToInt32(r["cliePedidos_Mensal"]),
                cliePedidos_Mes_Ref  = r["cliePedidos_Mes_Ref"] == DBNull.Value ? "" : r["cliePedidos_Mes_Ref"].ToString(),
                clieData_Cadastro    = r["clieData_Cadastro"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(r["clieData_Cadastro"]),
                Situacao             = r["Situacao"]?.ToString() ?? "A",
                Status_Transmissao   = r["Status_Transmissao"]?.ToString() ?? "N",
                Info                 = r["Info"]?.ToString() ?? "",
            };
        }

        /// <summary>Retorna todos os clientes ativos que possuem celular ou telefone cadastrado.</summary>
        public System.Collections.Generic.List<(string fone, string nome)> ListarComCelular()
        {
            var lista = new System.Collections.Generic.List<(string, string)>();
            using var conn = AbrirConexao();
            var sql = @"SELECT clieNome_RazaoSocial, clieCelular, clieTelefone
                        FROM cliente
                        WHERE Situacao = 'A'
                          AND (TRIM(COALESCE(clieCelular,'')) <> '' OR TRIM(COALESCE(clieTelefone,'')) <> '')
                        ORDER BY clieNome_RazaoSocial ASC";
            using var cmd = new MySqlCommand(sql, conn);
            using var r = cmd.ExecuteReader();
            while (r.Read())
            {
                string cel  = r["clieCelular"]?.ToString()  ?? "";
                string tel  = r["clieTelefone"]?.ToString() ?? "";
                string nome = r["clieNome_RazaoSocial"]?.ToString() ?? "";
                string fone = !string.IsNullOrWhiteSpace(cel) ? cel : tel;
                if (!string.IsNullOrWhiteSpace(fone) && !string.IsNullOrWhiteSpace(nome))
                    lista.Add((fone, nome));
            }
            return lista;
        }

        public void IncrementarTotais(int codigoCliente, decimal valorPedido)
        {
            if (codigoCliente <= 0) return;
            string mesAtual = DateTime.Today.ToString("yyyy-MM");
            using var conn = AbrirConexao();
            using var cmd = new MySqlCommand(
                @"UPDATE cliente SET
                    clieTotalPedidos = clieTotalPedidos + 1,
                    clieTotalGasto   = clieTotalGasto   + @val,
                    clieGasto_Mensal = CASE
                        WHEN clieGasto_Mes_Ref = @mes THEN clieGasto_Mensal + @val
                        ELSE @val
                    END,
                    clieGasto_Mes_Ref = @mes,
                    cliePedidos_Mensal = CASE
                        WHEN cliePedidos_Mes_Ref = @mes THEN cliePedidos_Mensal + 1
                        ELSE 1
                    END,
                    cliePedidos_Mes_Ref = @mes
                  WHERE Codigo = @cod", conn);
            cmd.Parameters.AddWithValue("@val", valorPedido);
            cmd.Parameters.AddWithValue("@mes", mesAtual);
            cmd.Parameters.AddWithValue("@cod", codigoCliente);
            cmd.ExecuteNonQuery();
        }
    }
}

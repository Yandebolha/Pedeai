using System;
using MySqlConnector;
using Pedeai.Modelo;

namespace Pedeai.DAL
{
    public class ConfiguracaoImpressaoDAL : BaseDAL
    {
        public ConfiguracaoImpressao Carregar()
        {
            using var conn = AbrirConexao();
            using var cmd  = new MySqlCommand(
                "SELECT * FROM config_impressao WHERE Codigo=1 LIMIT 1", conn);
            using var r = cmd.ExecuteReader();
            if (!r.Read()) return new ConfiguracaoImpressao();
            return new ConfiguracaoImpressao
            {
                cabNomeEmpresa    = r["cabNomeEmpresa"]?.ToString()   ?? "",
                cabEndereco       = r["cabEndereco"]?.ToString()      ?? "",
                cabTelefone       = r["cabTelefone"]?.ToString()      ?? "",
                cabCNPJ           = r["cabCNPJ"]?.ToString()          ?? "",
                separador         = r["separador"]?.ToString()        ?? "-",
                rodapeAvisoFiscal = r["rodapeAvisoFiscal"]?.ToString()  ?? "*** NAO E DOCUMENTO FISCAL ***",
                rodapeTextoLivre  = r["rodapeTextoLivre"]?.ToString()   ?? "",
                lblNumeroPedido   = r["lblNumeroPedido"]?.ToString()    ?? "Pedido N.:",
                lblColunaItem     = r["lblColunaItem"]?.ToString()      ?? "ITEM (V.Unit)",
                lblColunaTotal    = r["lblColunaTotal"]?.ToString()     ?? "Total",
                lblSubtotal       = r["lblSubtotal"]?.ToString()        ?? "TOTAL:",
                lblTaxaEntrega    = r["lblTaxaEntrega"]?.ToString()     ?? "+ ENTREGA:",
                lblTotalPagar     = r["lblTotalPagar"]?.ToString()      ?? "= TOTAL A PAGAR:",
                lblAtendente      = r["lblAtendente"]?.ToString()       ?? "Atendente:",
                larguraCaracteres = r["larguraCaracteres"] == DBNull.Value
                                      ? 42 : Convert.ToInt32(r["larguraCaracteres"]),
                impressoraNome    = r["impressoraNome"]?.ToString()     ?? "",
            };
        }

        public string Salvar(ConfiguracaoImpressao obj)
        {
            try
            {
                using var conn = AbrirConexao();
                const string sql = @"
                    INSERT INTO config_impressao
                        (Codigo, cabNomeEmpresa, cabEndereco, cabTelefone, cabCNPJ,
                         separador, rodapeAvisoFiscal, rodapeTextoLivre,
                         lblNumeroPedido, lblColunaItem, lblColunaTotal,
                         lblSubtotal, lblTaxaEntrega, lblTotalPagar, lblAtendente,
                         larguraCaracteres, impressoraNome)
                    VALUES
                        (1,@nome,@end,@tel,@cnpj,
                         @sep,@aviso,@livre,
                         @num,@col,@ctot,
                         @sub,@taxa,@total,@atend,
                         @larg,@impr)
                    ON DUPLICATE KEY UPDATE
                        cabNomeEmpresa    = VALUES(cabNomeEmpresa),
                        cabEndereco       = VALUES(cabEndereco),
                        cabTelefone       = VALUES(cabTelefone),
                        cabCNPJ           = VALUES(cabCNPJ),
                        separador         = VALUES(separador),
                        rodapeAvisoFiscal = VALUES(rodapeAvisoFiscal),
                        rodapeTextoLivre  = VALUES(rodapeTextoLivre),
                        lblNumeroPedido   = VALUES(lblNumeroPedido),
                        lblColunaItem     = VALUES(lblColunaItem),
                        lblColunaTotal    = VALUES(lblColunaTotal),
                        lblSubtotal       = VALUES(lblSubtotal),
                        lblTaxaEntrega    = VALUES(lblTaxaEntrega),
                        lblTotalPagar     = VALUES(lblTotalPagar),
                        lblAtendente      = VALUES(lblAtendente),
                        larguraCaracteres = VALUES(larguraCaracteres),
                        impressoraNome    = VALUES(impressoraNome)";

                using var cmd = new MySqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@nome",  obj.cabNomeEmpresa ?? "");
                cmd.Parameters.AddWithValue("@end",   obj.cabEndereco ?? "");
                cmd.Parameters.AddWithValue("@tel",   obj.cabTelefone ?? "");
                cmd.Parameters.AddWithValue("@cnpj",  obj.cabCNPJ ?? "");
                cmd.Parameters.AddWithValue("@sep",   obj.separador ?? "-");
                cmd.Parameters.AddWithValue("@aviso", obj.rodapeAvisoFiscal ?? "");
                cmd.Parameters.AddWithValue("@livre", obj.rodapeTextoLivre ?? "");
                cmd.Parameters.AddWithValue("@num",   obj.lblNumeroPedido ?? "Pedido N.:");
                cmd.Parameters.AddWithValue("@col",   obj.lblColunaItem ?? "ITEM (V.Unit)");
                cmd.Parameters.AddWithValue("@ctot",  obj.lblColunaTotal ?? "Total");
                cmd.Parameters.AddWithValue("@sub",   obj.lblSubtotal ?? "TOTAL:");
                cmd.Parameters.AddWithValue("@taxa",  obj.lblTaxaEntrega ?? "+ ENTREGA:");
                cmd.Parameters.AddWithValue("@total", obj.lblTotalPagar ?? "= TOTAL A PAGAR:");
                cmd.Parameters.AddWithValue("@atend", obj.lblAtendente ?? "Atendente:");
                cmd.Parameters.AddWithValue("@larg",  obj.larguraCaracteres);
                cmd.Parameters.AddWithValue("@impr",  obj.impressoraNome ?? "");
                cmd.ExecuteNonQuery();
                return "";
            }
            catch (Exception ex) { return ex.Message; }
        }
    }
}

using System;
using System.Security.Cryptography;
using System.Text;

namespace PedeaiLicencaServer.Services
{
    /// <summary>
    /// Lógica criptográfica de licença — idêntica ao LicencaService do cliente.
    /// IMPORTANTE: mantenha o valor de Segredo em sincronia com o sistema cliente.
    /// </summary>
    public static class LicencaService
    {
        // Deve ser EXATAMENTE igual ao const Segredo do Pedeai/DB/LicencaService.cs
        private const string Segredo = "PEDEAI-LIC-V1-2026";

        /// <summary>
        /// Gera a chave de ativação válida até a data de expiração informada.
        /// </summary>
        public static string GerarChave(string codigoEmpresa, DateTime expiracao)
        {
            if (string.IsNullOrWhiteSpace(codigoEmpresa))
                throw new ArgumentException("Código de empresa inválido.");

            string periodo = expiracao.ToString("yyyyMMdd");
            string input = codigoEmpresa.Trim().ToUpperInvariant() + "|" + periodo;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Segredo));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            string hex = BitConverter.ToString(hash).Replace("-", "").Substring(0, 15).ToUpper();
            return $"{periodo}-{hex.Substring(0, 5)}-{hex.Substring(5, 5)}-{hex.Substring(10, 5)}";
        }

        /// <summary>Valida a chave: verifica assinatura e prazo.</summary>
        public static bool ValidarChave(string codigoEmpresa, string chave)
        {
            if (string.IsNullOrWhiteSpace(codigoEmpresa) || string.IsNullOrWhiteSpace(chave))
                return false;

            string[] parts = chave.Trim().ToUpperInvariant().Replace(" ", "").Split('-');
            if (parts.Length != 4 || parts[0].Length != 8) return false;

            if (!int.TryParse(parts[0].Substring(0, 4), out int ano)) return false;
            if (!int.TryParse(parts[0].Substring(4, 2), out int mes)) return false;
            if (!int.TryParse(parts[0].Substring(6, 2), out int dia)) return false;
            if (mes < 1 || mes > 12 || dia < 1 || dia > 31) return false;

            DateTime expiracao;
            try { expiracao = new DateTime(ano, mes, dia); }
            catch { return false; }

            if (DateTime.Today > expiracao) return false;

            string esperada = GerarChave(codigoEmpresa, expiracao);
            return CryptographicEquals(
                esperada.Replace("-", ""),
                chave.Trim().ToUpperInvariant().Replace("-", ""));
        }

        private static bool CryptographicEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}

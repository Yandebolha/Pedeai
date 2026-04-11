using System;
using System.Security.Cryptography;
using System.Text;

namespace Pedeai.DB
{
    /// <summary>
    /// Gera e valida chaves de licença mensais para o sistema.
    ///
    /// Fluxo:
    ///  1. Cada instalação possui um Código de Empresa único (empCodigo_Empresa).
    ///  2. O operador informa esse código ao suporte junto com o mês de renovação.
    ///  3. O suporte usa o sistema externo (LicencaGenerator) para gerar a chave do mês.
    ///  4. O operador digita a chave na tela de ativação.
    ///  5. A cada inicialização o sistema valida se a chave ainda está no prazo.
    ///
    /// Formato da chave: YYYYMM-AAAAA-BBBBB-CCCCC
    ///   YYYYMM = ano+mês de expiração (último dia do mês)
    ///   AAAAA-BBBBB-CCCCC = 15 hex de HMAC-SHA256(codigoEmpresa|YYYYMM, segredo)
    /// </summary>
    public static class LicencaService
    {
        // Segredo compartilhado entre este sistema e o LicencaGenerator.
        // NÃO altere este valor após o primeiro deploy em produção.
        private const string Segredo = "PEDEAI-LIC-V1-2026";

        /// <summary>
        /// Gera um Código de Empresa aleatório (8 chars alfanumérico maiúsculo).
        /// Chamado uma única vez na migração, quando o campo está vazio.
        /// </summary>
        public static string GerarCodigoEmpresa()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // sem O/0/I/1 para evitar confusão
            var rng = RandomNumberGenerator.Create();
            var bytes = new byte[8];
            rng.GetBytes(bytes);
            var sb = new StringBuilder(8);
            foreach (var b in bytes)
                sb.Append(chars[b % chars.Length]);
            return sb.ToString();
        }

        /// <summary>
        /// Gera a chave de ativação válida até a data exata informada.
        /// Formato: YYYYMMDD-AAAAA-BBBBB-CCCCC
        /// </summary>
        public static string GerarChave(string codigoEmpresa, DateTime expiracao)
        {
            if (string.IsNullOrWhiteSpace(codigoEmpresa))
                throw new ArgumentException("Código de empresa inválido.");

            string periodo = expiracao.ToString("yyyyMMdd");
            string input   = codigoEmpresa.Trim().ToUpperInvariant() + "|" + periodo;
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(Segredo));
            byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            string hex = BitConverter.ToString(hash).Replace("-", "").Substring(0, 15).ToUpper();
            return $"{periodo}-{hex.Substring(0, 5)}-{hex.Substring(5, 5)}-{hex.Substring(10, 5)}";
        }

        /// <summary>
        /// Valida a chave: verifica assinatura e prazo de validade.
        /// Retorna false se a chave for inválida ou estiver expirada.
        /// </summary>
        public static bool ValidarChave(string codigoEmpresa, string chave)
        {
            if (string.IsNullOrWhiteSpace(codigoEmpresa) || string.IsNullOrWhiteSpace(chave))
                return false;

            string[] parts = chave.Trim().ToUpperInvariant().Replace(" ", "").Split('-');
            // Espera 4 segmentos: YYYYMMDD + 3 grupos de 5
            if (parts.Length != 4 || parts[0].Length != 8) return false;

            if (!int.TryParse(parts[0].Substring(0, 4), out int ano)) return false;
            if (!int.TryParse(parts[0].Substring(4, 2), out int mes)) return false;
            if (!int.TryParse(parts[0].Substring(6, 2), out int dia)) return false;
            if (mes < 1 || mes > 12 || dia < 1 || dia > 31) return false;

            DateTime expiracao;
            try { expiracao = new DateTime(ano, mes, dia); }
            catch { return false; }

            // Verifica prazo
            if (DateTime.Today > expiracao) return false;

            // Verifica assinatura
            string esperada = GerarChave(codigoEmpresa, expiracao);
            return CryptographicEquals(
                esperada.Replace("-", ""),
                chave.Trim().ToUpperInvariant().Replace("-", ""));
        }

        /// <summary>Extrai a data de expiração da chave, ou null se inválida.</summary>
        public static DateTime? ObterExpiracao(string chave)
        {
            if (string.IsNullOrWhiteSpace(chave)) return null;
            var parts = chave.Trim().ToUpperInvariant().Split('-');
            if (parts.Length != 4 || parts[0].Length != 8) return null;
            if (!int.TryParse(parts[0].Substring(0, 4), out int ano)) return null;
            if (!int.TryParse(parts[0].Substring(4, 2), out int mes)) return null;
            if (!int.TryParse(parts[0].Substring(6, 2), out int dia)) return null;
            if (mes < 1 || mes > 12 || dia < 1 || dia > 31) return null;
            try { return new DateTime(ano, mes, dia); }
            catch { return null; }
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

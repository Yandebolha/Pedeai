using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;

namespace PedeaiLicencaServer.Filters
{
    /// <summary>
    /// Filtro de autenticação por API Key. Lê o header "X-Api-Key" e compara
    /// com o valor configurado em appsettings.json → ApiKey.
    /// Use [ApiKeyAuth] no controller ou no action method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiKeyAuthAttribute : Attribute, IAuthorizationFilter
    {
        private const string HeaderName = "X-Api-Key";

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var config = context.HttpContext.RequestServices
                .GetService(typeof(IConfiguration)) as IConfiguration;
            string expectedKey = config?["ApiKey"] ?? string.Empty;

            if (string.IsNullOrWhiteSpace(expectedKey))
            {
                // Sem ApiKey configurada no servidor → rejeitar tudo
                context.Result = new ObjectResult("Servidor mal configurado (ApiKey ausente).")
                {
                    StatusCode = 500
                };
                return;
            }

            if (!context.HttpContext.Request.Headers.TryGetValue(HeaderName, out var receivedKey)
                || !CryptographicEquals(receivedKey.ToString().Trim(), expectedKey.Trim()))
            {
                context.Result = new UnauthorizedObjectResult("API key inválida.");
            }
        }

        /// <summary>Comparação de tempo constante para evitar timing attack.</summary>
        private static bool CryptographicEquals(string a, string b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            int diff = 0;
            for (int i = 0; i < a.Length; i++)
                diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}

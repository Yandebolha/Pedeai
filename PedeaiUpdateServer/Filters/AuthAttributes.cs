using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace PedeaiUpdateServer.Filters
{
    /// <summary>Autentica clientes via header X-Api-Key.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ClienteAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Permite ignorar com [IgnoreClientAuth]
            bool ignore = context.ActionDescriptor.EndpointMetadata
                .OfType<IgnoreClientAuthAttribute>().Any();
            if (ignore) return;

            var config = context.HttpContext.RequestServices
                .GetService(typeof(IConfiguration)) as IConfiguration;
            string expected = config?["ClienteApiKey"] ?? "";

            if (string.IsNullOrWhiteSpace(expected)) { context.Result = new StatusCodeResult(500); return; }

            if (!context.HttpContext.Request.Headers.TryGetValue("X-Api-Key", out var received)
                || !TimeConstantEquals(received.ToString().Trim(), expected.Trim()))
                context.Result = new UnauthorizedObjectResult("API key inválida.");
        }

        private static bool TimeConstantEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int d = 0; for (int i = 0; i < a.Length; i++) d |= a[i] ^ b[i]; return d == 0;
        }
    }

    /// <summary>Autentica administradores via header X-Admin-Token.</summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AdminAuthAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var config = context.HttpContext.RequestServices
                .GetService(typeof(IConfiguration)) as IConfiguration;
            string expected = config?["AdminToken"] ?? "";

            if (string.IsNullOrWhiteSpace(expected)) { context.Result = new StatusCodeResult(500); return; }

            if (!context.HttpContext.Request.Headers.TryGetValue("X-Admin-Token", out var received)
                || !TimeConstantEquals(received.ToString().Trim(), expected.Trim()))
                context.Result = new UnauthorizedObjectResult("Token de admin inválido.");
        }

        private static bool TimeConstantEquals(string a, string b)
        {
            if (a.Length != b.Length) return false;
            int d = 0; for (int i = 0; i < a.Length; i++) d |= a[i] ^ b[i]; return d == 0;
        }
    }

    /// <summary>Marca uma action para ignorar ClienteAuth (usada em endpoints públicos).</summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class IgnoreClientAuthAttribute : Attribute { }
}

using Microsoft.AspNetCore.Mvc;
using PedeaiLicencaServer.Filters;
using PedeaiLicencaServer.Models;
using PedeaiLicencaServer.Services;
using System;

namespace PedeaiLicencaServer.Controllers
{
    [ApiController]
    [Route("api/licenca")]
    public class LicencaController : ControllerBase
    {
        // ── GET /api/licenca/ping ────────────────────────────────────────────
        // Health-check público; não exige API Key.
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { status = "ok", timestamp = DateTime.UtcNow.ToString("o") });
        }

        // ── POST /api/licenca/renovar ────────────────────────────────────────
        // Gera uma nova chave de licença para o código de empresa informado.
        // Requer header: X-Api-Key com a chave configurada em appsettings.json.
        [HttpPost("renovar")]
        [ApiKeyAuth]
        public IActionResult Renovar([FromBody] RenovarRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.CodigoEmpresa))
                return BadRequest(new { erro = "CodigoEmpresa é obrigatório." });

            int dias = request.DiasValidade;
            if (dias < 1 || dias > 366) dias = 30;

            string codEmp = request.CodigoEmpresa.Trim().ToUpperInvariant();
            DateTime expiracao = DateTime.Today.AddDays(dias);

            string chave;
            try
            {
                chave = LicencaService.GerarChave(codEmp, expiracao);
            }
            catch (Exception ex)
            {
                return BadRequest(new { erro = $"Erro ao gerar chave: {ex.Message}" });
            }

            return Ok(new RenovarResponse
            {
                CodigoEmpresa = codEmp,
                Chave         = chave,
                Expiracao     = expiracao.ToString("yyyy-MM-dd")
            });
        }

        // ── POST /api/licenca/validar ────────────────────────────────────────
        // Valida se uma chave ainda está em vigor. Retorna 200 OK ou 422 Unprocessable.
        [HttpPost("validar")]
        [ApiKeyAuth]
        public IActionResult Validar([FromBody] ValidarRequest request)
        {
            if (request == null
                || string.IsNullOrWhiteSpace(request.CodigoEmpresa)
                || string.IsNullOrWhiteSpace(request.Chave))
            {
                return BadRequest(new { erro = "CodigoEmpresa e Chave são obrigatórios." });
            }

            bool valida = LicencaService.ValidarChave(request.CodigoEmpresa.Trim().ToUpperInvariant(), request.Chave.Trim());
            if (!valida)
                return UnprocessableEntity(new { valida = false, motivo = "Chave inválida ou expirada." });

            return Ok(new { valida = true });
        }
    }
}

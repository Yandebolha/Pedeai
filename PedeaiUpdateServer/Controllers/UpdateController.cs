using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PedeaiUpdateServer.Data;
using PedeaiUpdateServer.Filters;
using PedeaiUpdateServer.Models;
using System;
using System.IO;

namespace PedeaiUpdateServer.Controllers
{
    /// <summary>
    /// Endpoints para clientes (UpdateService nas instalações).
    /// Autenticação via X-Api-Key + X-Client-Id.
    /// </summary>
    [ApiController]
    [Route("api/update")]
    [ClienteAuth]
    public class UpdateController : ControllerBase
    {
        private readonly UpdateDb _db;
        private readonly string   _packagesDir;

        public UpdateController(UpdateDb db, string packagesDir)
        {
            _db          = db;
            _packagesDir = packagesDir;
        }

        // ── GET /api/update/ping ─────────────────────────────────────────────────
        // Sem autenticação (saúde do servidor)
        [HttpGet("/api/update/ping")]
        [IgnoreClientAuth]
        public IActionResult Ping() => Ok(new { status = "ok", timestamp = DateTime.UtcNow });

        // ── POST /api/update/registrar ───────────────────────────────────────────
        [HttpPost("registrar")]
        public IActionResult Registrar([FromBody] RegistrarClienteRequest req)
        {
            if (req == null || string.IsNullOrWhiteSpace(req.CodigoEmpresa))
                return BadRequest("CodigoEmpresa é obrigatório.");

            long id = _db.RegistrarCliente(req.CodigoEmpresa, req.NomeEmpresa);
            return Ok(new RegistrarClienteResponse { ClienteId = id, Mensagem = "Registrado." });
        }

        // ── GET /api/update/verificar ────────────────────────────────────────────
        [HttpGet("verificar")]
        public IActionResult Verificar([FromQuery] long clienteId, [FromQuery] string versaoAtual)
        {
            _db.AtualizarUltimaConsulta(clienteId);

            var cliente = _db.ObterCliente(clienteId);
            if (cliente == null) return NotFound("Cliente não registrado.");
            if (cliente.Bloqueado) return StatusCode(403, new { erro = "Cliente bloqueado." });

            var pacote = _db.ObterPacoteParaCliente(clienteId, versaoAtual ?? "");

            if (pacote == null)
                return Ok(new VerificarUpdateResponse { TemAtualizacao = false });

            return Ok(new VerificarUpdateResponse
            {
                TemAtualizacao = true,
                PacoteId       = pacote.Id,
                Versao         = pacote.Versao,
                Descricao      = pacote.Descricao,
                TemSQL         = pacote.TemSQL,
                TamanhoBytes   = pacote.TamanhoBytes
            });
        }

        // ── GET /api/update/download/{pacoteId} ──────────────────────────────────
        [HttpGet("download/{pacoteId}")]
        public IActionResult Download(long pacoteId, [FromQuery] long clienteId)
        {
            var pacote = _db.ObterPacote(pacoteId);
            if (pacote == null) return NotFound("Pacote não encontrado.");
            if (!System.IO.File.Exists(pacote.CaminhoArquivo))
                return StatusCode(500, "Arquivo de pacote ausente no servidor.");

            _db.RegistrarDownload(clienteId, pacoteId);

            var fileBytes = System.IO.File.ReadAllBytes(pacote.CaminhoArquivo);
            string fileName = Path.GetFileName(pacote.CaminhoArquivo);
            return File(fileBytes, "application/zip", fileName);
        }

        // ── POST /api/update/confirmar/{pacoteId} ────────────────────────────────
        [HttpPost("confirmar/{pacoteId}")]
        public IActionResult Confirmar(long pacoteId, [FromQuery] long clienteId,
                                       [FromBody] ConfirmarUpdateRequest req)
        {
            _db.ConfirmarAplicacao(clienteId, pacoteId,
                req?.Status ?? "aplicado", req?.Detalhe);
            return Ok();
        }
    }
}

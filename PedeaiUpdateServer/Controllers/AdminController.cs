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
    /// Endpoints de administração: publicar pacotes, gerenciar clientes.
    /// Autenticação via X-Admin-Token.
    /// </summary>
    [ApiController]
    [Route("api/admin")]
    [AdminAuth]
    public class AdminController : ControllerBase
    {
        private readonly UpdateDb _db;
        private readonly string   _packagesDir;

        public AdminController(UpdateDb db, string packagesDir)
        {
            _db          = db;
            _packagesDir = packagesDir;
        }

        // ── Pacotes ───────────────────────────────────────────────────────────────

        /// <summary>Upload de pacote. Multipart com campos: arquivo(zip), versao, nivel, descricao.</summary>
        [HttpPost("pacotes/publicar")]
        [RequestSizeLimit(500_000_000)]  // 500 MB
        public IActionResult Publicar([FromForm] IFormFile arquivo,
                                      [FromForm] string versao,
                                      [FromForm] int nivel = 2,
                                      [FromForm] string descricao = "")
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo não enviado.");
            if (string.IsNullOrWhiteSpace(versao))
                return BadRequest("Versão é obrigatória.");
            if (nivel < 1 || nivel > 3)
                return BadRequest("Nível deve ser 1 (Beta), 2 (Standard) ou 3 (Legacy).");

            Directory.CreateDirectory(_packagesDir);
            string nomeArquivo = $"update_v{versao}_n{nivel}_{DateTime.UtcNow:yyyyMMddHHmmss}.zip";
            string caminho     = Path.Combine(_packagesDir, nomeArquivo);

            using (var fs = new FileStream(caminho, FileMode.Create, FileAccess.Write))
                arquivo.CopyTo(fs);

            // Verifica se o zip contém update.sql
            bool temSql = false;
            try
            {
                using var zip = System.IO.Compression.ZipFile.OpenRead(caminho);
                foreach (var entry in zip.Entries)
                    if (entry.Name.Equals("update.sql", StringComparison.OrdinalIgnoreCase))
                    { temSql = true; break; }
            }
            catch { /* zip inválido — retorna erro abaixo */ }

            long id = _db.InserirPacote(new Pacote
            {
                Versao         = versao.Trim(),
                Nivel          = nivel,
                Descricao      = descricao ?? "",
                CaminhoArquivo = caminho,
                TamanhoBytes   = new FileInfo(caminho).Length,
                TemSQL         = temSql
            });

            return Ok(new PublicarResponse { PacoteId = id, Versao = versao, Mensagem = "Publicado." });
        }

        [HttpGet("pacotes")]
        public IActionResult ListarPacotes() => Ok(_db.ListarPacotes());

        [HttpDelete("pacotes/{id}")]
        public IActionResult ExcluirPacote(long id)
        {
            _db.DesativarPacote(id);
            return Ok();
        }

        // ── Clientes ──────────────────────────────────────────────────────────────

        [HttpGet("clientes")]
        public IActionResult ListarClientes() => Ok(_db.ListarClientes());

        [HttpPut("clientes/{id}/nivel")]
        public IActionResult AlterarNivel(long id, [FromBody] AlterarNivelRequest req)
        {
            if (req == null || req.Nivel < 1 || req.Nivel > 3)
                return BadRequest("Nível inválido (1=Beta, 2=Standard, 3=Legacy).");
            _db.AlterarNivelCliente(id, req.Nivel);
            return Ok();
        }

        [HttpPut("clientes/{id}/bloquear")]
        public IActionResult Bloquear(long id)
        {
            _db.AlterarBloqueioCliente(id, true);
            return Ok();
        }

        [HttpPut("clientes/{id}/desbloquear")]
        public IActionResult Desbloquear(long id)
        {
            _db.AlterarBloqueioCliente(id, false);
            return Ok();
        }
    }
}

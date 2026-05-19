using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Produtos.Midias;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Route("api/produtos/{produtoId:int}/midias")]
    public class ProdutoMidiasController : ControllerBase
    {
        private readonly ProdutoMidiaService _produtoMidiaService;

        public ProdutoMidiasController(ProdutoMidiaService produtoMidiaService)
        {
            _produtoMidiaService = produtoMidiaService;
        }

        [HttpGet]
        public async Task<IActionResult> Listar(int produtoId)
        {
            return Ok(await _produtoMidiaService.ListarAsync(produtoId));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UploadMidias(int produtoId, [FromForm] List<IFormFile> arquivos)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var resposta = await _produtoMidiaService.UploadMidiasAsync(produtoId, usuarioId, arquivos);
                return Ok(resposta);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                if (ex.Message == "Envie ao menos 1 arquivo.")
                    return BadRequest(ex.Message);

                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Remove uma midia do produto e apaga o arquivo fisico quando ele foi salvo localmente.
        [HttpDelete("{midiaId:int}")]
        [Authorize]
        public async Task<IActionResult> Remover(int produtoId, int midiaId)
        {
            try
            {
                var usuarioId = User.GetUserId();
                await _produtoMidiaService.RemoverAsync(produtoId, midiaId, usuarioId);
                return Ok(new { mensagem = "Midia removida com sucesso." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }
    }
}

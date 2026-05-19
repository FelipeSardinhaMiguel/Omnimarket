using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Produtos.Avaliacoes;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Route("api/produtos/{produtoId:int}/avaliacoes")]
    public class ProdutoAvaliacoesController : ControllerBase
    {
        private readonly AvaliacaoProdutoService _service;

        public ProdutoAvaliacoesController(AvaliacaoProdutoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Listar(int produtoId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Ok(await _service.ListarPorProdutoAsync(produtoId, page, pageSize));

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Criar(int produtoId, [FromBody] ProdutoAvaliacaoCriacaoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioId = User.GetUserId();
                var avaliacao = await _service.CriarAsync(produtoId, usuarioId, dto);
                return CreatedAtAction(nameof(Listar), new { produtoId, id = avaliacao.Id }, avaliacao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{avaliacaoId:int}")]
        public async Task<IActionResult> Atualizar(int produtoId, int avaliacaoId, [FromBody] ProdutoAvaliacaoAtualizacaoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioId = User.GetUserId();
                var avaliacao = await _service.AtualizarAsync(produtoId, avaliacaoId, usuarioId, dto);

                if (avaliacao == null)
                    return NotFound(new { mensagem = "Avaliacao nao encontrada." });

                return Ok(avaliacao);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}

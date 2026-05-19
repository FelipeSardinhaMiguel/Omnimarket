using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Produtos;
using Omnimarket.Api.Services.Interfaces;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _service;

        public ProdutoController(IProdutoService service)
        {
            _service = service;
        }

        // Lista todos os produtos disponiveis.
        [HttpGet]
        public async Task<IActionResult> Get()
            => Ok(await _service.GetAllAsync());

        // Busca um produto especifico pelo identificador.
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var produto = await _service.GetByIdAsync(id);

            if (produto == null)
                return NotFound();

            return Ok(produto);
        }

        // Cria um produto na loja do usuario autenticado.
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] ProdutoCriacaoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioId = User.GetUserId();
                var produto = await _service.CreateAsync(dto, usuarioId);

                return CreatedAtAction(nameof(Get), new { id = produto.Id }, produto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Atualiza os dados principais do produto simples do usuario logado.
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Put(int id, [FromBody] ProdutoAtualizarDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioId = User.GetUserId();
                var updated = await _service.UpdateAsync(id, dto, usuarioId);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Atualiza apenas o estoque do produto do usuario logado.
        [HttpPut("{id:int}/estoque")]
        [Authorize]
        public async Task<IActionResult> AtualizarEstoque(int id, [FromBody] ProdutoAtualizarEstoqueDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuarioId = User.GetUserId();
                var updated = await _service.AtualizarEstoqueAsync(id, dto, usuarioId);

                if (!updated)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Desativa um produto, sem remover do banco, para preservar pedidos antigos.
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var deleted = await _service.DeleteAsync(id, usuarioId);

                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // Retorna uma lista paginada com filtros simples de nome e faixa de preco.
        [HttpGet("filtro")]
        public async Task<IActionResult> GetPaged([FromQuery] ProdutoFiltroDto filtro)
        {
            var result = await _service.GetPagedAsync(filtro);
            return Ok(result);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Pedidos.Carrinho;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omni.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/carrinho")]
    public class CarrinhoController : ControllerBase
    {
        private readonly CarrinhoService _carrinhoService;

        public CarrinhoController(CarrinhoService carrinhoService)
        {
            _carrinhoService = carrinhoService;
        }

        // Retorna o carrinho completo do usuario logado, com itens, totais e imagens.
        [HttpGet]
        public async Task<IActionResult> ObterCarrinho()
        {
            var usuarioId = User.GetUserId();
            return Ok(await _carrinhoService.ObterCarrinhoAsync(usuarioId));
        }

        // Adiciona um produto ao carrinho do usuario, respeitando estoque e disponibilidade.
        [HttpPost]
        public async Task<IActionResult> AdicionarItem([FromBody] CarrinhoAdicionarDto dto)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var carrinho = await _carrinhoService.AdicionarItemAsync(usuarioId, dto);
                return Ok(carrinho);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Atualiza a quantidade de um produto que ja esta no carrinho do usuario.
        [HttpPut("{produtoId:int}")]
        public async Task<IActionResult> AtualizarQuantidade(int produtoId, [FromBody] CarrinhoAtualizarQuantidadeDto dto)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var carrinho = await _carrinhoService.AtualizarQuantidadeAsync(usuarioId, produtoId, dto);
                return Ok(carrinho);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Remove um produto especifico do carrinho do usuario logado.
        [HttpDelete("{produtoId:int}")]
        public async Task<IActionResult> RemoverItem(int produtoId)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var carrinho = await _carrinhoService.RemoverItemAsync(usuarioId, produtoId);
                return Ok(carrinho);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        // Remove todos os itens do carrinho do usuario logado.
        [HttpDelete]
        public async Task<IActionResult> LimparCarrinho()
        {
            var usuarioId = User.GetUserId();
            var limpou = await _carrinhoService.LimparCarrinhoAsync(usuarioId);

            if (!limpou)
                return Ok(new { mensagem = "Carrinho ja esta vazio." });

            return Ok(new
            {
                mensagem = "Carrinho limpo com sucesso.",
                itens = new List<object>(),
                totalItens = 0,
                valorTotal = 0m
            });
        }
    }
}

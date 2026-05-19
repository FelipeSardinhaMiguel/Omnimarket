using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Produtos.Lojas;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Route("api/lojas")]
    public class LojasController : ControllerBase
    {
        private readonly LojaService _lojaService;
        private readonly ReciboPedidoService _reciboPedidoService;

        public LojasController(LojaService lojaService, ReciboPedidoService reciboPedidoService)
        {
            _lojaService = lojaService;
            _reciboPedidoService = reciboPedidoService;
        }

        // Retorna a loja vinculada ao usuario autenticado.
        [Authorize]
        [HttpGet("minha")]
        public async Task<IActionResult> ObterMinhaLoja()
        {
            var usuarioId = User.GetUserId();
            var loja = await _lojaService.ObterMinhaLojaAsync(usuarioId);

            if (loja == null)
                return NotFound(new { mensagem = "Loja ainda nao cadastrada para este usuario." });

            return Ok(loja);
        }

        [Authorize]
        [HttpGet("minha/metricas")]
        public async Task<IActionResult> ObterMinhasMetricas()
        {
            var usuarioId = User.GetUserId();
            var metricas = await _lojaService.ObterMinhasMetricasAsync(usuarioId);

            if (metricas == null)
                return NotFound(new { mensagem = "Loja ainda nao cadastrada para este usuario." });

            return Ok(metricas);
        }

        [Authorize]
        [HttpGet("minha/pedidos/{pedidoId:int}/recibo")]
        public async Task<IActionResult> BaixarReciboPedidoLoja(int pedidoId)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var pdf = await _reciboPedidoService.GerarReciboPedidoParaVendedorAsync(pedidoId, usuarioId);

                if (pdf == null)
                    return NotFound(new { mensagem = "Pedido nao encontrado para a sua loja." });

                return File(
                    pdf,
                    "application/pdf",
                    $"recibo-pedido-{pedidoId}-loja.pdf");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Cria a loja do usuario autenticado.
        [Authorize]
        [HttpPost("minha")]
        public async Task<IActionResult> CriarMinhaLoja([FromBody] LojaCriacaoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = User.GetUserId();
                var loja = await _lojaService.CriarMinhaLojaAsync(usuarioId, dto);

                return CreatedAtAction(nameof(ObterPorSlug), new { slug = loja.Slug }, loja);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Atualiza a loja do usuario autenticado.
        [Authorize]
        [HttpPut("minha")]
        public async Task<IActionResult> AtualizarMinhaLoja([FromBody] LojaAtualizacaoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = User.GetUserId();
                var loja = await _lojaService.AtualizarMinhaLojaAsync(usuarioId, dto);

                if (loja == null)
                    return NotFound(new { mensagem = "Loja nao encontrada para este usuario." });

                return Ok(loja);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Endpoint publico para consultar a loja pelo slug.
        [HttpGet("{slug}")]
        public async Task<IActionResult> ObterPorSlug(string slug)
        {
            var loja = await _lojaService.ObterPorSlugAsync(slug);

            if (loja == null)
                return NotFound(new { mensagem = "Loja nao encontrada." });

            return Ok(loja);
        }
    }
}

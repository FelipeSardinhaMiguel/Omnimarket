using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Financeiro;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/financeiro")]
    public class FinanceiroController : ControllerBase
    {
        private readonly FinanceiroService _financeiroService;

        public FinanceiroController(FinanceiroService financeiroService)
        {
            _financeiroService = financeiroService;
        }

        [HttpPost("pagamentos/iniciar")]
        public async Task<IActionResult> IniciarPagamento([FromBody] IniciarPagamentoDto dto)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var resultado = await _financeiroService.IniciarPagamentoAsync(usuarioId, dto);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPost("pagamentos/{planoPagamentoId:int}/confirmar-fake")]
        public async Task<IActionResult> ConfirmarPagamentoFake(int planoPagamentoId)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var resultado = await _financeiroService.ConfirmarPagamentoFakeAsync(usuarioId, planoPagamentoId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Produtos.Lojas.Entregas;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Route("api/lojas")]
    public class LojaEntregasController : ControllerBase
    {
        private readonly LojaEntregaService _lojaEntregaService;

        public LojaEntregasController(LojaEntregaService lojaEntregaService)
        {
            _lojaEntregaService = lojaEntregaService;
        }

        [Authorize]
        [HttpGet("minha/entregas")]
        public async Task<IActionResult> ListarMinhas()
        {
            var usuarioId = User.GetUserId();
            var opcoes = await _lojaEntregaService.ListarMinhasOpcoesAsync(usuarioId);

            return Ok(opcoes);
        }

        [HttpGet("{lojaId:int}/entregas")]
        public async Task<IActionResult> ListarPublicas(
            int lojaId,
            [FromQuery] string? cep,
            [FromQuery] string? cidade,
            [FromQuery] string? uf)
        {
            var opcoes = await _lojaEntregaService.ListarOpcoesPublicasAsync(lojaId, cep, cidade, uf);
            return Ok(opcoes);
        }

        [Authorize]
        [HttpPost("minha/entregas")]
        public async Task<IActionResult> Criar([FromBody] LojaEntregaOpcaoCriacaoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = User.GetUserId();
                var opcao = await _lojaEntregaService.CriarMinhaOpcaoAsync(usuarioId, dto);

                return CreatedAtAction(nameof(ListarMinhas), new { id = opcao.Id }, opcao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("minha/entregas/{id:int}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] LojaEntregaOpcaoAtualizacaoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = User.GetUserId();
                var opcao = await _lojaEntregaService.AtualizarMinhaOpcaoAsync(usuarioId, id, dto);

                if (opcao == null)
                    return NotFound(new { mensagem = "Opcao de entrega nao encontrada." });

                return Ok(opcao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("minha/entregas/{id:int}")]
        public async Task<IActionResult> Remover(int id)
        {
            var usuarioId = User.GetUserId();
            var removido = await _lojaEntregaService.RemoverMinhaOpcaoAsync(usuarioId, id);

            if (!removido)
                return NotFound(new { mensagem = "Opcao de entrega nao encontrada." });

            return Ok(new { mensagem = "Opcao de entrega removida com sucesso." });
        }
    }
}

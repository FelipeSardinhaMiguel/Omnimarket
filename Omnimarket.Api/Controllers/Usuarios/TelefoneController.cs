using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Usuarios.Telefones;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/telefones")]
    public class TelefonesController : ControllerBase
    {
        private readonly TelefoneService _telefoneService;

        public TelefonesController(TelefoneService telefoneService)
        {
            _telefoneService = telefoneService;
        }

        // Lista os telefones vinculados ao usuario logado.
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var usuarioId = User.GetUserId();
            return Ok(await _telefoneService.ListarAsync(usuarioId));
        }

        // Busca um telefone especifico pertencente ao usuario autenticado.
        [HttpGet("{telefoneId:int}")]
        public async Task<IActionResult> Obter(int telefoneId)
        {
            var usuarioId = User.GetUserId();
            var telefone = await _telefoneService.ObterAsync(usuarioId, telefoneId);

            return telefone is null
                ? NotFound(new { mensagem = "Telefone nao encontrado." })
                : Ok(telefone);
        }

        // Cria um novo telefone validando o formato brasileiro antes de salvar.
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] UsuarioTelefoneDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = User.GetUserId();
                await _telefoneService.CriarAsync(usuarioId, dto);

                return Ok(new { mensagem = "Telefone cadastrado com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Atualiza o telefone e opcionalmente redefine qual e o principal.
        [HttpPut("{telefoneId:int}")]
        public async Task<IActionResult> Atualizar(int telefoneId, [FromBody] UsuarioTelefoneDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var usuarioId = User.GetUserId();
                var atualizado = await _telefoneService.AtualizarAsync(usuarioId, telefoneId, dto);
                if (!atualizado)
                    return NotFound(new { mensagem = "Telefone nao encontrado." });

                return Ok(new { mensagem = "Telefone atualizado com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        // Remove um telefone preservando pelo menos um telefone cadastrado.
        [HttpDelete("{telefoneId:int}")]
        public async Task<IActionResult> Remover(int telefoneId)
        {
            try
            {
                var usuarioId = User.GetUserId();
                var removido = await _telefoneService.RemoverAsync(usuarioId, telefoneId);
                if (!removido)
                    return NotFound(new { mensagem = "Telefone nao encontrado." });

                return Ok(new { mensagem = "Telefone removido com sucesso." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}

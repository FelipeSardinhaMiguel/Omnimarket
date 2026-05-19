using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Usuarios.Enderecos;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Route("api/usuarios/{usuarioId:int}/enderecos")]
    [Authorize]
    public class EnderecosController : ControllerBase
    {
        private readonly EnderecoService _enderecoService;

        public EnderecosController(EnderecoService enderecoService)
        {
            _enderecoService = enderecoService;
        }

        // Lista os enderecos do usuario logado.
        // Por padrao so traz os ativos, mas pode incluir os inativos na consulta.
        [HttpGet]
        public async Task<IActionResult> Listar(int usuarioId, [FromQuery] bool incluirInativos = false)
        {
            if (usuarioId != User.GetUserId())
                return Forbid();

            return Ok(await _enderecoService.ListarAsync(usuarioId, incluirInativos));
        }

        // Busca um endereco especifico do usuario logado.
        // Tambem consegue consultar inativos quando incluirInativos for true.
        [HttpGet("{enderecoId:int}")]
        public async Task<IActionResult> Obter(int usuarioId, int enderecoId, [FromQuery] bool incluirInativos = false)
        {
            if (usuarioId != User.GetUserId())
                return Forbid();

            var endereco = await _enderecoService.ObterAsync(usuarioId, enderecoId, incluirInativos);

            return endereco is null
                ? NotFound(new { mensagem = "Endereco nao encontrado." })
                : Ok(endereco);
        }

        // Cria um novo endereco para o usuario autenticado.
        [HttpPost]
        public async Task<IActionResult> Criar(int usuarioId, [FromBody] UsuarioEnderecoDto dto)
        {
            var usuarioIdLogado = User.GetUserId();

            if (usuarioId != usuarioIdLogado)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var endereco = await _enderecoService.CriarAsync(usuarioIdLogado, dto);

                return CreatedAtAction(
                    nameof(Obter),
                    new { usuarioId = usuarioIdLogado, enderecoId = endereco.Id },
                    endereco);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        // Atualiza um endereco ativo do usuario autenticado.
        [HttpPut("{enderecoId:int}")]
        public async Task<IActionResult> Atualizar(int usuarioId, int enderecoId, [FromBody] UsuarioEnderecoDto dto)
        {
            var usuarioIdLogado = User.GetUserId();

            if (usuarioId != usuarioIdLogado)
                return Forbid();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var endereco = await _enderecoService.AtualizarAsync(usuarioIdLogado, enderecoId, dto);
            if (endereco is null)
                return NotFound(new { mensagem = "Endereco nao encontrado." });

            return Ok(endereco);
        }

        // O delete do endereco e logico.
        // Em vez de apagar o registro, ele fica inativo para consultas futuras.
        [HttpDelete("{enderecoId:int}")]
        public async Task<IActionResult> Remover(int usuarioId, int enderecoId)
        {
            var usuarioIdLogado = User.GetUserId();

            if (usuarioId != usuarioIdLogado)
                return Forbid();

            var removido = await _enderecoService.RemoverAsync(usuarioIdLogado, enderecoId);
            if (!removido)
                return NotFound(new { mensagem = "Endereco nao encontrado." });

            return Ok(new { mensagem = "Endereco desativado com sucesso." });
        }

        // Lista os valores possiveis do enum de tipos de logradouro para o front-end.
        [HttpGet("tipos-logradouro")]
        public IActionResult GetTiposLogradouro()
        {
            return Ok(_enderecoService.ObterTiposLogradouro());
        }
    }
}

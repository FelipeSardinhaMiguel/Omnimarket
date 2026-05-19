using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Usuarios;
using Omnimarket.Api.Services;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly RegistrarService _registrarService;
        private readonly UsuarioPerfilService _usuarioPerfilService;

        public UsuarioController(RegistrarService registrarService, UsuarioPerfilService usuarioPerfilService)
        {
            _registrarService = registrarService;
            _usuarioPerfilService = usuarioPerfilService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userId = User.GetUserId();
            var usuario = await _usuarioPerfilService.ObterPerfilAsync(userId);

            if (usuario is null)
                return NotFound();

            return Ok(usuario);
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarUsuario([FromBody] UsuarioRegistroComContatoDto userDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var usuario = await _registrarService.RegistrarUsuario(userDto);

                return Ok(new
                {
                    mensagem = "Usuario registrado com sucesso!",
                    usuario = new
                    {
                        id = usuario.Id,
                        nome = $"{usuario.Nome} {usuario.Sobrenome}",
                        email = usuario.Email
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> AtualizarUsuario(int id, [FromBody] UsuarioAtualizarDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioIdLogado = User.GetUserId();
            if (id != usuarioIdLogado)
                return Forbid();

            try
            {
                var usuario = await _usuarioPerfilService.AtualizarAsync(id, dto);
                if (usuario == null)
                    return NotFound(new { mensagem = "Usuario nao encontrado." });

                return Ok(new
                {
                    mensagem = "Usuario atualizado com sucesso!",
                    usuario = new
                    {
                        usuario.Id,
                        usuario.Nome,
                        usuario.Sobrenome,
                        usuario.Email
                    }
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }
}

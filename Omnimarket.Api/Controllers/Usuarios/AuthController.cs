using Microsoft.AspNetCore.Mvc;
using Omnimarket.Api.Models.Dtos.Usuarios.Autenticacao;
using Omnimarket.Api.Services;

namespace Omnimarket.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var result = await _authService.Login(login);

                if (result == null)
                {
                    return Unauthorized(new
                    {
                        mensagem = "Email ou senha incorretos"
                    });
                }

                return Ok(new
                {
                    mensagem = "Login realizado com sucesso",
                    token = result.Token,
                    tokenExpiraEm = result.TokenExpiraEm,
                    usuario = new
                    {
                        nome = result.Nome,
                        email = result.Email,
                        role = result.Role
                    }
                });
            }
            catch (Exception)
            {
                return StatusCode(500, new
                {
                    mensagem = "Erro interno ao realizar login"
                });
            }
        }
    }
}

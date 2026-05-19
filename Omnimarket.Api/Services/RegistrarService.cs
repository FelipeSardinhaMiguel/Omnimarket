using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Usuarios;

namespace Omnimarket.Api.Services
{
    // Mantem o fluxo de registro separado no controller sem duplicar a logica do AuthService.
    public class RegistrarService
    {
        private readonly AuthService _authService;

        public RegistrarService(AuthService authService)
        {
            _authService = authService;
        }

        public Task<Usuario> RegistrarUsuario(UsuarioRegistroComContatoDto userDto)
        {
            return _authService.RegistrarUsuario(userDto);
        }
    }
}

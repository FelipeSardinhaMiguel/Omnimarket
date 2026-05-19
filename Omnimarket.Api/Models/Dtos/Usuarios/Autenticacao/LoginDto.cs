using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Usuarios.Autenticacao
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Email e obrigatorio")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha e obrigatoria")]
        public string Password { get; set; } = string.Empty;
    }
}

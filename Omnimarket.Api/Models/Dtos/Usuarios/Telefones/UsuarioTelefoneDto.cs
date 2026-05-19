using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Usuarios.Telefones
{
    public class UsuarioTelefoneDto
    {
        [Required(ErrorMessage = "DDD e obrigatorio.")]
        [RegularExpression(@"^\d{2}$", ErrorMessage = "DDD deve conter 2 digitos.")]
        public string Ddd { get; set; } = string.Empty;

        [Required(ErrorMessage = "Numero e obrigatorio.")]
        [RegularExpression(@"^\d{8,9}$", ErrorMessage = "Numero deve conter 8 ou 9 digitos.")]
        public string Numero { get; set; } = string.Empty;

        public bool? IsPrincipal { get; set; }
    }
}

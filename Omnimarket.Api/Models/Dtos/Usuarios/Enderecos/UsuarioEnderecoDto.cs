using System.ComponentModel.DataAnnotations;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Usuarios.Enderecos
{
    public class UsuarioEnderecoDto
    {
        [Required(ErrorMessage = "CEP é obrigatório.")]
        [StringLength(8, MinimumLength = 8, ErrorMessage = "CEP deve conter 8 dígitos.")]
        public string Cep { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "Tipo de logradouro é obrigatório.")]
        public TiposLogradouroBR TipoLogradouro { get; set; }

        [Required(ErrorMessage = "Nome do endereço é obrigatório.")]
        [StringLength(200)]
        public string NomeEndereco { get; set; } = string.Empty;

        [Required(ErrorMessage = "Número é obrigatório.")]
        [StringLength(20)]
        public string Numero { get; set; } = string.Empty;

        [StringLength(80)]
        public string? Complemento { get; set; }

        [Required(ErrorMessage = "Cidade é obrigatória.")]
        [StringLength(120)]
        public string Cidade { get; set; } = string.Empty;

        [Required(ErrorMessage = "UF é obrigatória.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "UF deve ter 2 caracteres.")]
        public string Uf { get; set; } = string.Empty;

        public bool? IsPrincipal { get; set; }
    }
}

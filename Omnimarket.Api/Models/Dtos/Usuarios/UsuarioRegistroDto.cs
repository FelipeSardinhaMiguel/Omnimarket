using System.ComponentModel.DataAnnotations;
using Omnimarket.Api.Models.Dtos.Usuarios.Enderecos;
using Omnimarket.Api.Models.Dtos.Usuarios.Telefones;

namespace Omnimarket.Api.Models.Dtos.Usuarios
{
    public class UsuarioRegistroDto
    {
        [Required(ErrorMessage = "CPF e obrigatorio")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter 11 digitos numericos")]
        public string Cpf { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nome e obrigatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        public string Nome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Sobrenome e obrigatorio")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Sobrenome deve ter entre 2 e 100 caracteres")]
        public string Sobrenome { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email e obrigatorio")]
        [EmailAddress(ErrorMessage = "Email invalido")]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha e obrigatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Senha deve ter no minimo 6 caracteres")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmacao de senha e obrigatoria")]
        [Compare("Password", ErrorMessage = "As senhas nao coincidem")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Voce deve aceitar os termos.")]
        [Range(typeof(bool), "true", "true", ErrorMessage = "Voce deve aceitar os termos.")]
        public bool AceitouTermos { get; set; }
    }

    public class UsuarioRegistroComContatoDto : UsuarioRegistroDto
    {
        [Required(ErrorMessage = "Informe pelo menos 1 telefone.")]
        [MinLength(1, ErrorMessage = "Informe pelo menos 1 telefone.")]
        public List<UsuarioTelefoneDto> Telefones { get; set; } = new();

        public List<UsuarioEnderecoDto> Enderecos { get; set; } = new();
    }
}

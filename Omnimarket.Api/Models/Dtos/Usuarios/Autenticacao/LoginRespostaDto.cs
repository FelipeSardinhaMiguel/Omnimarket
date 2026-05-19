namespace Omnimarket.Api.Models.Dtos.Usuarios.Autenticacao
{
    public class LoginRespostaDto
    {
        public string Token { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime TokenExpiraEm { get; set; }
    }
}

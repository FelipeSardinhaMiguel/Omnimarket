namespace Omnimarket.Api.Models.Dtos.Usuarios
{
    public class UsuarioPerfilLeituraDto
    {
        public int Id { get; set; }
        public string Cpf { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public List<UsuarioPerfilTelefoneLeituraDto> Telefones { get; set; } = new();
        public List<UsuarioPerfilEnderecoLeituraDto> Enderecos { get; set; } = new();
    }
}

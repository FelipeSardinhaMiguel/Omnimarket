namespace Omnimarket.Api.Models.Dtos.Usuarios.Enderecos
{
    public class UsuarioEnderecoLeituraDto
    {
        public int Id { get; set; }
        public string TipoLogradouro { get; set; } = string.Empty;
        public string NomeEndereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Cep { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public bool IsPrincipal { get; set; }
        public bool Ativo { get; set; }
    }
}

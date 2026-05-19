using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Usuarios
{
    public class UsuarioPerfilEnderecoLeituraDto
    {
        public int Id { get; set; }
        public TiposLogradouroBR TipoLogradouro { get; set; }
        public string NomeEndereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Cidade { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public bool IsPrincipal { get; set; }
        public bool Ativo { get; set; }
    }
}

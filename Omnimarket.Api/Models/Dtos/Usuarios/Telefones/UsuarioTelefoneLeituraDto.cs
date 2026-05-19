namespace Omnimarket.Api.Models.Dtos.Usuarios.Telefones
{
    public class UsuarioTelefoneLeituraDto
    {
        public int Id { get; set; }
        public string Numero { get; set; } = string.Empty;
        public bool IsPrincipal { get; set; }
    }
}

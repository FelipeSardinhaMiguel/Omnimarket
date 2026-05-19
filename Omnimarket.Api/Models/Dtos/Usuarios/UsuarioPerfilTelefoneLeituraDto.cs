namespace Omnimarket.Api.Models.Dtos.Usuarios
{
    public class UsuarioPerfilTelefoneLeituraDto
    {
        public int Id { get; set; }
        public string NumeroE164 { get; set; } = string.Empty;
        public bool IsPrincipal { get; set; }
    }
}

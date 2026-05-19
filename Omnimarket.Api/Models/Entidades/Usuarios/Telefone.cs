using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omnimarket.Api.Models
{
    public class Telefone
    {
        [Key]
        public int Id { get; set; }

        // 🔗 Relacionamento com usuário
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        // 📞 Número no padrão internacional
        [Required]
        [StringLength(15, MinimumLength = 10)]
        public string NumeroE164 { get; set; } = string.Empty; // ex: +5511912345678

        // ⭐ Telefone principal
        public bool IsPrincipal { get; set; } = false;
    }
}
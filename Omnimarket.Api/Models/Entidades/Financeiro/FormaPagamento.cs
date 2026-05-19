using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Entidades
{
    public class FormaPagamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;

        public bool Ativo { get; set; } = true;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        [StringLength(300)]
        public string Observacao { get; set; } = string.Empty;
    }
}

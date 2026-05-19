using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Omnimarket.Api.Models.Entidades
{
    public class LojaEntregaOpcao
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LojaId { get; set; }

        [ForeignKey(nameof(LojaId))]
        [JsonIgnore]
        public Loja Loja { get; set; } = null!;

        [Required(ErrorMessage = "Tipo de entrega e obrigatorio.")]
        public int TipoEntregaId { get; set; }

        [Required]
        [StringLength(120)]
        public string Nome { get; set; } = string.Empty;

        [Range(0, double.MaxValue, ErrorMessage = "Valor de frete invalido.")]
        public decimal ValorFrete { get; set; }

        [Range(0, 365, ErrorMessage = "Prazo de entrega invalido.")]
        public int PrazoEntregaDias { get; set; }

        [StringLength(500)]
        public string? Observacao { get; set; }

        public bool Ativa { get; set; } = true;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime? DataAtualizacao { get; set; }
    }
}

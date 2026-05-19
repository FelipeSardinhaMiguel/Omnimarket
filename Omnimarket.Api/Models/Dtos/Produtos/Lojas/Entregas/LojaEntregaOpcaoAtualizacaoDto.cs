using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Produtos.Lojas.Entregas
{
    public class LojaEntregaOpcaoAtualizacaoDto
    {
        [Required]
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
    }
}

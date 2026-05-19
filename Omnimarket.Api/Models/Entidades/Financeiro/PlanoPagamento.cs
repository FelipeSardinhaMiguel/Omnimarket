using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Entidades
{
    public class PlanoPagamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [ForeignKey("PedidoId")]
        public Pedido Pedido { get; set; } = null!;

        [Required]
        public int FormaPagamentoId { get; set; }

        [ForeignKey(nameof(FormaPagamentoId))]
        public FormaPagamento? FormaPagamento { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ValorTotal { get; set; }

        [Required]
        public StatusPagamento StatusPagamento { get; set; } = StatusPagamento.Pendente;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}

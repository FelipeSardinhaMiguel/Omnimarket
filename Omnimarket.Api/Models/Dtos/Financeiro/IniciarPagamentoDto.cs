using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Financeiro
{
    public class IniciarPagamentoDto
    {
        [Range(1, int.MaxValue)]
        public int PedidoId { get; set; }

        [Range(1, int.MaxValue)]
        public int FormaPagamentoId { get; set; }

        [StringLength(500)]
        public string? Observacao { get; set; }
    }
}

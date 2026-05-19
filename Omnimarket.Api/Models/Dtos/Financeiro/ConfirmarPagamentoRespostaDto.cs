using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Financeiro
{
    public class ConfirmarPagamentoRespostaDto
    {
        public int PlanoPagamentoId { get; set; }
        public int PedidoId { get; set; }
        public int FormaPagamentoId { get; set; }
        public StatusPagamento StatusPagamento { get; set; }
        public string GatewayTransactionId { get; set; } = string.Empty;
        public DateTime? DataConfirmacao { get; set; }
        public int QuantidadeVendasAtualizadas { get; set; }
    }
}

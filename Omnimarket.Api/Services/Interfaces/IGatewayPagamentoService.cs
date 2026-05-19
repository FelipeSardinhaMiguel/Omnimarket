using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Services.Interfaces
{
    public class GatewayPagamentoRetorno
    {
        public StatusPagamento StatusPagamento { get; set; }
        public string GatewayTransactionId { get; set; } = string.Empty;
        public string CodigoAutorizacao { get; set; } = string.Empty;
    }

    public interface IGatewayPagamentoService
    {
        Task<GatewayPagamentoRetorno> ProcessarPagamentoAsync(decimal valorTotal, int planoPagamentoId);
    }
}

using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Services.Interfaces;

namespace Omnimarket.Api.Services
{
    // Simulacao simples de gateway para o MVP do TCC.
    public class GatewayPagamentoFakeService : IGatewayPagamentoService
    {
        public Task<GatewayPagamentoRetorno> ProcessarPagamentoAsync(decimal valorTotal, int planoPagamentoId)
        {
            if (valorTotal <= 0)
                throw new Exception("Valor total para processar pagamento e invalido.");

            var retorno = new GatewayPagamentoRetorno
            {
                StatusPagamento = StatusPagamento.Aprovado,
                GatewayTransactionId = $"FAKE_TX_{planoPagamentoId}_{DateTime.UtcNow:yyyyMMddHHmmss}",
                CodigoAutorizacao = $"AUTH{DateTime.UtcNow:HHmmss}"
            };

            return Task.FromResult(retorno);
        }
    }
}

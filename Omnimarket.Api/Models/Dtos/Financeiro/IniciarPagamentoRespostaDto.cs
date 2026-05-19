using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Financeiro
{
    public class IniciarPagamentoRespostaDto
    {
        public int PedidoId { get; set; }
        public int PlanoPagamentoId { get; set; }
        public int FormaPagamentoId { get; set; }
        public decimal ValorTotal { get; set; }
        public StatusPagamento StatusPagamento { get; set; }
        public List<VendaFinanceiraResumoDto> Vendas { get; set; } = new();
    }

    public class VendaFinanceiraResumoDto
    {
        public int VendaId { get; set; }
        public int VendedorId { get; set; }
        public decimal ValorBruto { get; set; }
        public decimal ValorComissao { get; set; }
        public decimal ValorLiquido { get; set; }
        public StatusVenda StatusVenda { get; set; }
    }
}

using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Produtos.Lojas
{
    public class LojaMetricasResumoDto
    {
        public int LojaId { get; set; }
        public string NomeFantasia { get; set; } = string.Empty;
        public decimal FaturamentoBruto { get; set; }
        public decimal FaturamentoLiquido { get; set; }
        public decimal TicketMedio { get; set; }
        public double TaxaCancelamento { get; set; }
        public double MediaAvaliacao { get; set; }
        public int TotalAvaliacoes { get; set; }
        public List<LojaMetricaPedidoStatusDto> PedidosPorStatus { get; set; } = new();
        public List<LojaMetricaProdutoDto> ProdutosMaisVendidosPorQuantidade { get; set; } = new();
        public List<LojaMetricaProdutoDto> ProdutosMaisVendidosPorReceita { get; set; } = new();
    }

    public class LojaMetricaPedidoStatusDto
    {
        public StatusPedido Status { get; set; }
        public int Total { get; set; }
    }

    public class LojaMetricaProdutoDto
    {
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public int QuantidadeVendida { get; set; }
        public decimal ReceitaBruta { get; set; }
    }
}

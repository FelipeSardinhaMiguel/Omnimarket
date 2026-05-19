namespace Omnimarket.Api.Models.Dtos.Pedidos.Recibos
{
    public class ReciboPedidoDto
    {
        public int PedidoId { get; set; }
        public DateTime DataPedido { get; set; }
        public string TipoRecibo { get; set; } = "Comprador";
        public string StatusPedido { get; set; } = string.Empty;
        public string NomeComprador { get; set; } = string.Empty;
        public string EmailComprador { get; set; } = string.Empty;
        public string EnderecoEntrega { get; set; } = string.Empty;
        public decimal ValorProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorTotal { get; set; }
        public string FormaPagamento { get; set; } = string.Empty;
        public string StatusPagamento { get; set; } = string.Empty;
        public List<ReciboPedidoItemDto> Itens { get; set; } = new();
    }

    public class ReciboPedidoItemDto
    {
        public string NomeProduto { get; set; } = string.Empty;
        public string NomeLoja { get; set; } = string.Empty;
        public string DocumentoLoja { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}

using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Pedidos
{
    public class PedidoLeituraDto
    {
        public int Id { get; set; }
        public StatusPedido Status { get; set; }
        public string TipoEntrega { get; set; } = string.Empty;
        public decimal ValorTotalProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorTotalPedido { get; set; }
        public DateTime DataPedido { get; set; }
        public string Observacao { get; set; } = string.Empty;
        public string TipoLogradouroEntrega { get; set; } = string.Empty;
        public string NomeEnderecoEntrega { get; set; } = string.Empty;
        public string NumeroEntrega { get; set; } = string.Empty;
        public string? ComplementoEntrega { get; set; }
        public string CepEntrega { get; set; } = string.Empty;
        public string CidadeEntrega { get; set; } = string.Empty;
        public string UfEntrega { get; set; } = string.Empty;
        public List<ItemPedidoLeituraDto> Itens { get; set; } = new();
    }

    public class ItemPedidoLeituraDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public string SkuProduto { get; set; } = string.Empty;
        public int LojaId { get; set; }
        public string NomeLoja { get; set; } = string.Empty;
        public string SlugLoja { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal ValorTotal { get; set; }
    }
}

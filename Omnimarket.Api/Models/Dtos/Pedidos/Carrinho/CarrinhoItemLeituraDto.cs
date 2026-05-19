using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Pedidos.Carrinho
{
    public class CarrinhoItemLeituraDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public int LojaId { get; set; }
        public string NomeLoja { get; set; } = string.Empty;
        public string SlugLoja { get; set; } = string.Empty;
        public int Quantidade { get; set; }
        public decimal PrecoUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public int EstoqueDisponivel { get; set; }
        public StatusProduto StatusPublicacao { get; set; }
        public bool DisponivelParaCompra { get; set; }
        public string? ImagemPrincipal { get; set; }
    }
}

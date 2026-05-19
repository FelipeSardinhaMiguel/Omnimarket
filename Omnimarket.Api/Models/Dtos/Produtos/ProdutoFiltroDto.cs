namespace Omnimarket.Api.Models.Dtos.Produtos
{
    public class ProdutoFiltroDto
    {
        public string? Nome { get; set; }
        public string? Categoria { get; set; }
        public int? LojaId { get; set; }
        public decimal? PrecoMinimo { get; set; }
        public decimal? PrecoMaximo { get; set; }
        public bool? Disponivel { get; set; }
        public Omnimarket.Api.Models.Enum.StatusProduto? StatusPublicacao { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}

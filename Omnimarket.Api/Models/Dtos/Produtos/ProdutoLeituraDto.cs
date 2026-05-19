using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Produtos
{
    public class ProdutoLeituraDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public bool Disponivel { get; set; }
        public StatusProduto StatusPublicacao { get; set; }
        public string? Descricao { get; set; }
        public double MediaAvaliacao { get; set; }
        public int TotalAvaliacoes { get; set; }
        public DateTimeOffset DtCriacao { get; set; }
        public DateTimeOffset? DtAtualizacao { get; set; }
        public int LojaId { get; set; }
        public string NomeLoja { get; set; } = string.Empty;
        public string SlugLoja { get; set; } = string.Empty;
        public List<string> Imagens { get; set; } = new();
    }
}

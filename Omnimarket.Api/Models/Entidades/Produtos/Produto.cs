using System.ComponentModel.DataAnnotations;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Entidades
{
    public class Produto
    {
        public int Id { get; set; }

        [Required]
        public int LojaId { get; set; }

        public Loja Loja { get; set; } = null!;

        [Required, StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required, StringLength(100)]
        public string Categoria { get; set; } = string.Empty;

        [Required, StringLength(40)]
        public string Sku { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue)]
        public decimal Preco { get; set; }

        [Range(0, int.MaxValue)]
        public int Estoque { get; set; }

        public bool Disponivel => StatusPublicacao == StatusProduto.Publicado && Estoque > 0;

        [StringLength(1000)]
        public string? Descricao { get; set; }

        [Required]
        public StatusProduto StatusPublicacao { get; set; } = StatusProduto.Publicado;

        public double MediaAvaliacao { get; set; }
        public int TotalAvaliacoes { get; set; }

        public DateTimeOffset DtCriacao { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DtAtualizacao { get; set; }

        public ICollection<ProdutoMidia> Midias { get; set; } = new List<ProdutoMidia>();
        public ICollection<AvaliacaoProduto> Avaliacoes { get; set; } = new List<AvaliacaoProduto>();
    }
}

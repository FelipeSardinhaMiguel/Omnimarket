using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Omnimarket.Api.Models.Entidades
{
    [Index(nameof(ProdutoId), nameof(PedidoId), nameof(UsuarioId), IsUnique = true)]
    public class AvaliacaoProduto
    {
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        [Required]
        public int LojaId { get; set; }
        public Loja Loja { get; set; } = null!;

        [Required]
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; } = null!;

        [Required]
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        [Range(1, 5)]
        public int NotaProduto { get; set; }

        [Range(1, 5)]
        public int NotaLoja { get; set; }

        [StringLength(120)]
        public string? Titulo { get; set; }

        [StringLength(1200)]
        public string? Comentario { get; set; }

        public bool CompraVerificada { get; set; } = true;
        public bool RecomendaProduto { get; set; } = true;

        public DateTimeOffset DtCriacao { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DtAtualizacao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Produtos.Avaliacoes
{
    public class ProdutoAvaliacaoAtualizacaoDto
    {
        [Range(1, 5)]
        public int NotaProduto { get; set; }

        [Range(1, 5)]
        public int? NotaLoja { get; set; }

        [StringLength(120)]
        public string? Titulo { get; set; }

        [StringLength(1200)]
        public string? Comentario { get; set; }

        public bool RecomendaProduto { get; set; } = true;
    }
}

using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Produtos
{
    public class ProdutoAtualizarEstoqueDto
    {
        [Range(0, int.MaxValue, ErrorMessage = "Estoque nao pode ser negativo.")]
        public int Estoque { get; set; }
    }
}

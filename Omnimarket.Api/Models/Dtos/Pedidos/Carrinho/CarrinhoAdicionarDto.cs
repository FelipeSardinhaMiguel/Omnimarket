using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Pedidos.Carrinho
{
    public class CarrinhoAdicionarDto
    {
        [Required]
        public int ProdutoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }
    }
}

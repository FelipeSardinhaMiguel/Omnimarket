using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Pedidos.Carrinho
{
    public class CarrinhoAtualizarQuantidadeDto
    {
        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }
    }
}

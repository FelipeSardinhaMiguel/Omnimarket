using System.ComponentModel.DataAnnotations;
using Omnimarket.Api.Models.Dtos.Pedidos.ItemPedido;

namespace Omnimarket.Api.Models.Dtos.Pedidos
{
    public class PedidoDto
    {
        public int? EnderecoId { get; set; }

        [Required]
        public int TipoEntregaId { get; set; }

        public string Observacao { get; set; } = string.Empty;

        public List<ItemPedidoDto> Itens { get; set; } = new();
    }
}

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Omnimarket.Api.Models.Dtos.Pedidos.ItemPedido
{
    public class ItemPedidoDto
    {
        [Required]
        public int ProdutoId { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantidade { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? QtdItens
        {
            get => Quantidade > 0 ? Quantidade : null;
            set
            {
                if (value.HasValue)
                    Quantidade = value.Value;
            }
        }
    }
}

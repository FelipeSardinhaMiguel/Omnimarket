using System.Text.Json.Serialization;

namespace Omnimarket.Api.Models.Dtos.Pedidos.Carrinho
{
    public class CarrinhoLeituraDto
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? CarrinhoId { get; set; }

        public List<CarrinhoItemLeituraDto> Itens { get; set; } = new();
        public int TotalItens { get; set; }
        public decimal ValorTotal { get; set; }
    }
}

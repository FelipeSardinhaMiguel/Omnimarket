using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Omnimarket.Api.Models.Entidades
{
    public class ItensPedido
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [ForeignKey(nameof(PedidoId))]
        public Pedido Pedido { get; set; } = null!;

        [Required]
        public int ProdutoId { get; set; }

        [ForeignKey(nameof(ProdutoId))]
        public Produto Produto { get; set; } = null!;

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantidade deve ser maior que 0.")]
        public int Quantidade { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preco invalido.")]
        public decimal PrecoUnitario { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ValorTotal { get; set; }

        [Required]
        [StringLength(150)]
        public string NomeProdutoSnapshot { get; set; } = string.Empty;

        [Required]
        [StringLength(150)]
        public string NomeLojaSnapshot { get; set; } = string.Empty;

        [Required]
        [StringLength(18)]
        public string DocumentoLojaSnapshot { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string TipoDocumentoLojaSnapshot { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Entidades
{
    public class Venda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PedidoId { get; set; }

        [ForeignKey("PedidoId")]
        public Pedido Pedido { get; set; } = null!;

        [Required]
        public int VendedorId { get; set; }

        [ForeignKey("VendedorId")]
        public Usuario Vendedor { get; set; } = null!;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal ValorBruto { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal ValorLiquido { get; set; }

        [Required]
        public StatusVenda StatusVenda { get; set; } = StatusVenda.Criada;

        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

        public DateTime? DataAtualizacao { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Entidades
{
    public class Pedido
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Usuario e obrigatorio.")]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string TipoLogradouroEntrega { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string NomeEnderecoEntrega { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string NumeroEntrega { get; set; } = string.Empty;

        [StringLength(80)]
        public string? ComplementoEntrega { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string CepEntrega { get; set; } = string.Empty;

        [Required]
        [StringLength(120)]
        public string CidadeEntrega { get; set; } = string.Empty;

        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string UfEntrega { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tipo de entrega e obrigatorio.")]
        public int TipoEntregaId { get; set; }

        [Required]
        public StatusPedido StatusPedidosId { get; set; } = StatusPedido.Pendente;

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Valor dos produtos invalido.")]
        public decimal ValorTotalProdutos { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Valor do frete invalido.")]
        public decimal ValorFrete { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Valor total invalido.")]
        public decimal ValorTotalPedido { get; set; }

        public DateTime DataPedido { get; set; } = DateTime.UtcNow;

        [StringLength(500)]
        public string Observacao { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "O pedido deve ter pelo menos 1 item.")]
        public List<ItensPedido> Itens { get; set; } = new();
    }
}

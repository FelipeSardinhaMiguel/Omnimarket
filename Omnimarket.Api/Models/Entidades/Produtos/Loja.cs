using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Entidades
{
    [Index(nameof(UsuarioId), IsUnique = true)]
    [Index(nameof(Slug), IsUnique = true)]
    [Index(nameof(TipoDocumentoFiscal), nameof(DocumentoFiscal), IsUnique = true)]
    public class Loja
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        [Required]
        [StringLength(120)]
        public string NomeFantasia { get; set; } = string.Empty;

        [Required]
        [StringLength(160)]
        public string Slug { get; set; } = string.Empty;

        [Required]
        public TipoDocumentoFiscalLoja TipoDocumentoFiscal { get; set; }

        [Required]
        [StringLength(18, MinimumLength = 11)]
        public string DocumentoFiscal { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [EmailAddress]
        [StringLength(120)]
        public string? EmailContato { get; set; }

        public int? EnderecoId { get; set; }

        [ForeignKey(nameof(EnderecoId))]
        public Endereco? Endereco { get; set; }

        public int? TelefoneId { get; set; }

        [ForeignKey(nameof(TelefoneId))]
        public Telefone? Telefone { get; set; }

        public bool Ativa { get; set; } = true;

        public double MediaAvaliacao { get; set; }
        public int TotalAvaliacoes { get; set; }

        public DateTimeOffset DtCriacao { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? DtAtualizacao { get; set; }

        public List<Produto> Produtos { get; set; } = new();
        public List<LojaEntregaOpcao> OpcoesEntrega { get; set; } = new();
        public List<AvaliacaoProduto> Avaliacoes { get; set; } = new();
    }
}

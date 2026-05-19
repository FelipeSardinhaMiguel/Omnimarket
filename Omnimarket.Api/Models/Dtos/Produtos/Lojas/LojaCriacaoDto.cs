using System.ComponentModel.DataAnnotations;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Produtos.Lojas
{
    public class LojaCriacaoDto
    {
        [Required]
        [StringLength(120, MinimumLength = 3)]
        public string NomeFantasia { get; set; } = string.Empty;

        [StringLength(160)]
        public string? Slug { get; set; }

        [Required]
        public TipoDocumentoFiscalLoja TipoDocumentoFiscal { get; set; }

        [Required(ErrorMessage = "Documento fiscal e obrigatorio.")]
        [StringLength(18)]
        public string DocumentoFiscal { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Descricao { get; set; }

        [EmailAddress]
        [StringLength(120)]
        public string? EmailContato { get; set; }

        public bool UsarEnderecoUsuario { get; set; }

        public int? EnderecoUsuarioId { get; set; }

        public EnderecoCriacaoDto? NovoEnderecoLoja { get; set; }

        public bool UsarTelefoneUsuario { get; set; }

        public int? TelefoneUsuarioId { get; set; }

        public TelefoneCriacaoDto? NovoTelefoneLoja { get; set; }

        public bool Ativa { get; set; } = true;
    }
}

using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Produtos.Lojas
{
    public class LojaGestaoLeituraDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeFantasia { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public TipoDocumentoFiscalLoja TipoDocumentoFiscal { get; set; }
        public string DocumentoFiscal { get; set; } = string.Empty;
        public string DocumentoFiscalFormatado { get; set; } = string.Empty;
        public string? Descricao { get; set; }
        public string? EmailContato { get; set; }
        public int? EnderecoId { get; set; }
        public string? Cep { get; set; }
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public string? NomeEndereco { get; set; }
        public string? NumeroEndereco { get; set; }
        public string? ComplementoEndereco { get; set; }
        public int? TelefoneId { get; set; }
        public string? NumeroTelefone { get; set; }
        public string? TipoTelefone { get; set; }
        public bool Ativa { get; set; }
        public double MediaAvaliacao { get; set; }
        public int TotalAvaliacoes { get; set; }
        public DateTimeOffset DtCriacao { get; set; }
        public DateTimeOffset? DtAtualizacao { get; set; }
    }
}

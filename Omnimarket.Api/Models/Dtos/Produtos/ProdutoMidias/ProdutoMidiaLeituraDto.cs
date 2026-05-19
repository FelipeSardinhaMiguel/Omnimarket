using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Produtos.Midias
{
    public class ProdutoMidiaLeituraDto
    {
        public int Id { get; set; }
        public TipoMidiaProduto Tipo { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? ContentType { get; set; }
        public int Ordem { get; set; }
    }
}

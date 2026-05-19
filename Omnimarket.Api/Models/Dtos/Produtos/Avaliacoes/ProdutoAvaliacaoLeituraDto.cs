namespace Omnimarket.Api.Models.Dtos.Produtos.Avaliacoes
{
    public class ProdutoAvaliacaoLeituraDto
    {
        public int Id { get; set; }
        public int ProdutoId { get; set; }
        public string NomeProduto { get; set; } = string.Empty;
        public int LojaId { get; set; }
        public string NomeLoja { get; set; } = string.Empty;
        public string SlugLoja { get; set; } = string.Empty;
        public int PedidoId { get; set; }
        public int UsuarioId { get; set; }
        public string NomeComprador { get; set; } = string.Empty;
        public int NotaProduto { get; set; }
        public int NotaLoja { get; set; }
        public string? Titulo { get; set; }
        public string? Comentario { get; set; }
        public bool CompraVerificada { get; set; }
        public bool RecomendaProduto { get; set; }
        public DateTimeOffset DtCriacao { get; set; }
        public DateTimeOffset? DtAtualizacao { get; set; }
    }
}

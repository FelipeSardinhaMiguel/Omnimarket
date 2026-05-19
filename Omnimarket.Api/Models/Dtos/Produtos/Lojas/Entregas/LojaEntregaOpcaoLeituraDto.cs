namespace Omnimarket.Api.Models.Dtos.Produtos.Lojas.Entregas
{
    public class LojaEntregaOpcaoLeituraDto
    {
        public int Id { get; set; }
        public int LojaId { get; set; }
        public string TipoEntrega { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public decimal ValorFrete { get; set; }
        public int PrazoEntregaDias { get; set; }
        public string? Observacao { get; set; }
        public bool Ativa { get; set; }
        public string ResumoCobertura { get; set; } = string.Empty;
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }
}

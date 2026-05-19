using System.ComponentModel.DataAnnotations;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Dtos.Admin
{
    public class AdminDashboardDto
    {
        public int TotalUsuarios { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalLojas { get; set; }
        public int TotalLojasAtivas { get; set; }
        public int TotalProdutos { get; set; }
        public int ProdutosPublicados { get; set; }
        public int TotalPedidos { get; set; }
        public int PedidosPendentes { get; set; }
        public int PedidosPagos { get; set; }
        public decimal ReceitaTotalMarketplace { get; set; }
    }

    public class AdminUsuarioDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime DataCadastro { get; set; }
        public DateTime? DataAcesso { get; set; }
        public bool PossuiLoja { get; set; }
        public bool LojaAtiva { get; set; }
        public int TotalProdutos { get; set; }
        public int TotalPedidos { get; set; }
    }

    public class AdminLojaDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeFantasia { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Cidade { get; set; }
        public string? Uf { get; set; }
        public bool Ativa { get; set; }
        public string NomeResponsavel { get; set; } = string.Empty;
        public string EmailResponsavel { get; set; } = string.Empty;
        public int TotalProdutos { get; set; }
        public int ProdutosPublicados { get; set; }
        public DateTimeOffset DtCriacao { get; set; }
        public DateTimeOffset? DtAtualizacao { get; set; }
    }

    public class AdminProdutoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Categoria { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public decimal Preco { get; set; }
        public int Estoque { get; set; }
        public bool Disponivel { get; set; }
        public StatusProduto StatusPublicacao { get; set; }
        public int UsuarioId { get; set; }
        public string NomeVendedor { get; set; } = string.Empty;
        public int? LojaId { get; set; }
        public string? NomeLoja { get; set; }
        public DateTimeOffset DtCriacao { get; set; }
        public DateTimeOffset? DtAtualizacao { get; set; }
    }

    public class AdminPedidoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NomeCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public StatusPedido Status { get; set; }
        public decimal ValorTotalProdutos { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorTotalPedido { get; set; }
        public int QuantidadeItens { get; set; }
        public DateTime DataPedido { get; set; }
        public string CidadeEntrega { get; set; } = string.Empty;
        public string UfEntrega { get; set; } = string.Empty;
    }

    public class AdminVendaFinanceiraDto
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public int VendedorId { get; set; }
        public string NomeVendedor { get; set; } = string.Empty;
        public decimal ValorBruto { get; set; }
        public decimal ValorComissao { get; set; }
        public decimal ValorLiquido { get; set; }
        public StatusVenda StatusVenda { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
    }

    public class AdminAtualizarRoleDto
    {
        [Required]
        public string Role { get; set; } = string.Empty;
    }

    public class AdminAtualizarStatusLojaDto
    {
        public bool Ativa { get; set; }
    }

    public class AdminAtualizarStatusProdutoDto
    {
        [Required]
        public StatusProduto StatusPublicacao { get; set; }
    }
}

using Microsoft.EntityFrameworkCore;
using Omni.Models.Entidades;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Entidades;

namespace Omnimarket.Api.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Usuario> TBL_USUARIO { get; set; }
        public DbSet<Endereco> TBL_ENDERECO { get; set; }
        public DbSet<Telefone> TBL_TELEFONE { get; set; }
        public DbSet<Produto> TBL_PRODUTO { get; set; }
        public DbSet<AvaliacaoProduto> TBL_AVALIACAO_PRODUTO { get; set; }
        public DbSet<ProdutoMidia> ProdutoMidia => Set<ProdutoMidia>();
        public DbSet<Pedido> TBL_PEDIDO { get; set; }
        public DbSet<ItensPedido> TBL_ITENS_PEDIDO { get; set; }
        public DbSet<Carrinho> TBL_CARRINHO { get; set; }
        public DbSet<ItemCarrinho> TBL_ITEM_CARRINHO { get; set; }
        public DbSet<Loja> TBL_LOJA { get; set; }
        public DbSet<LojaEntregaOpcao> TBL_LOJA_ENTREGA_OPCAO { get; set; }
        public DbSet<FormaPagamento> TBL_FORMA_PAGAMENTO { get; set; }
        public DbSet<PlanoPagamento> TBL_PLANO_PAGAMENTO { get; set; }
        public DbSet<Venda> TBL_VENDA { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().ToTable("TBL_USUARIO");
            modelBuilder.Entity<Endereco>().ToTable("TBL_ENDERECO");
            modelBuilder.Entity<Telefone>().ToTable("TBL_TELEFONE");
            modelBuilder.Entity<Produto>().ToTable("TBL_PRODUTOS");
            modelBuilder.Entity<AvaliacaoProduto>().ToTable("TBL_AVALIACAO_PRODUTO");
            modelBuilder.Entity<ProdutoMidia>().ToTable("TBL_PRODUTOS_MIDIA");
            modelBuilder.Entity<Pedido>().ToTable("TBL_PEDIDO");
            modelBuilder.Entity<ItensPedido>().ToTable("TBL_ITENS_PEDIDO");
            modelBuilder.Entity<Carrinho>().ToTable("TBL_CARRINHO");
            modelBuilder.Entity<ItemCarrinho>().ToTable("TBL_ITEM_CARRINHO");
            modelBuilder.Entity<Loja>().ToTable("TBL_LOJA");
            modelBuilder.Entity<LojaEntregaOpcao>().ToTable("TBL_LOJA_ENTREGA_OPCAO");
            modelBuilder.Entity<FormaPagamento>().ToTable("TBL_FORMA_PAGAMENTO");
            modelBuilder.Entity<PlanoPagamento>().ToTable("TBL_PLANO_PAGAMENTO");
            modelBuilder.Entity<Venda>().ToTable("TBL_VENDA");

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Telefones)
                .WithOne(t => t.Usuario)
                .HasForeignKey(t => t.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .HasMany(u => u.Enderecos)
                .WithOne(e => e.Usuario)
                .HasForeignKey(e => e.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Loja)
                .WithOne(l => l.Usuario)
                .HasForeignKey<Loja>(l => l.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Loja>()
                .HasOne(l => l.Endereco)
                .WithMany()
                .HasForeignKey(l => l.EnderecoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loja>()
                .HasOne(l => l.Telefone)
                .WithMany()
                .HasForeignKey(l => l.TelefoneId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Loja>()
                .HasMany(l => l.Produtos)
                .WithOne(p => p.Loja)
                .HasForeignKey(p => p.LojaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Loja>()
                .HasMany(l => l.OpcoesEntrega)
                .WithOne(e => e.Loja)
                .HasForeignKey(e => e.LojaId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Usuario>().HasIndex(x => x.Cpf).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(x => x.Email).IsUnique();
            modelBuilder.Entity<Carrinho>().HasIndex(c => c.UsuarioId).IsUnique();
            modelBuilder.Entity<Produto>().HasIndex(p => new { p.LojaId, p.Sku }).IsUnique();
            modelBuilder.Entity<Loja>().HasIndex(l => l.UsuarioId).IsUnique();
            modelBuilder.Entity<Loja>().HasIndex(l => l.Slug).IsUnique();
            modelBuilder.Entity<Loja>().HasIndex(l => new { l.TipoDocumentoFiscal, l.DocumentoFiscal }).IsUnique();
            modelBuilder.Entity<FormaPagamento>().HasIndex(f => f.Nome).IsUnique();
            modelBuilder.Entity<PlanoPagamento>().HasIndex(p => p.PedidoId).IsUnique();

            modelBuilder.Entity<Endereco>()
                .Property(e => e.TipoLogradouro)
                .HasConversion<string>();

            modelBuilder.Entity<Produto>()
                .Property(p => p.StatusPublicacao)
                .HasConversion<string>();

            modelBuilder.Entity<Produto>()
                .Property(p => p.Nome)
                .HasMaxLength(100);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Categoria)
                .HasMaxLength(100);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Sku)
                .HasMaxLength(40);

            modelBuilder.Entity<Produto>()
                .Property(p => p.Descricao)
                .HasMaxLength(1000);

            modelBuilder.Entity<Loja>()
                .Property(l => l.TipoDocumentoFiscal)
                .HasConversion<string>();

            modelBuilder.Entity<PlanoPagamento>()
                .Property(p => p.StatusPagamento)
                .HasConversion<string>();

            modelBuilder.Entity<Venda>()
                .Property(v => v.StatusVenda)
                .HasConversion<string>();

            modelBuilder.Entity<Produto>()
                .HasMany(p => p.Midias)
                .WithOne(m => m.Produto)
                .HasForeignKey(m => m.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Produto>()
                .HasMany(p => p.Avaliacoes)
                .WithOne(a => a.Produto)
                .HasForeignKey(a => a.ProdutoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Loja>()
                .HasMany(l => l.Avaliacoes)
                .WithOne(a => a.Loja)
                .HasForeignKey(a => a.LojaId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AvaliacaoProduto>()
                .HasOne(a => a.Pedido)
                .WithMany()
                .HasForeignKey(a => a.PedidoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AvaliacaoProduto>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Carrinho>()
                .HasMany(c => c.Itens)
                .WithOne(i => i.Carrinho)
                .HasForeignKey(i => i.CarrinhoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Itens)
                .WithOne(i => i.Pedido)
                .HasForeignKey(i => i.PedidoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ItensPedido>()
                .Property(i => i.NomeProdutoSnapshot)
                .HasMaxLength(150);

            modelBuilder.Entity<ItensPedido>()
                .Property(i => i.NomeLojaSnapshot)
                .HasMaxLength(150);

            modelBuilder.Entity<ItensPedido>()
                .Property(i => i.DocumentoLojaSnapshot)
                .HasMaxLength(18);

            modelBuilder.Entity<ItensPedido>()
                .Property(i => i.TipoDocumentoLojaSnapshot)
                .HasMaxLength(30);

            modelBuilder.Entity<PlanoPagamento>()
                .HasOne(p => p.Pedido)
                .WithMany()
                .HasForeignKey(p => p.PedidoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<PlanoPagamento>()
                .HasOne(p => p.FormaPagamento)
                .WithMany()
                .HasForeignKey(p => p.FormaPagamentoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Pedido)
                .WithMany()
                .HasForeignKey(v => v.PedidoId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Venda>()
                .HasOne(v => v.Vendedor)
                .WithMany()
                .HasForeignKey(v => v.VendedorId)
                .OnDelete(DeleteBehavior.NoAction);

            var dataSeedPadrao = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            modelBuilder.Entity<FormaPagamento>().HasData(
                new FormaPagamento
                {
                    Id = 1,
                    Nome = "Pix",
                    Ativo = true,
                    DataCriacao = dataSeedPadrao,
                    Observacao = "Pagamento instantaneo via Pix."
                },
                new FormaPagamento
                {
                    Id = 2,
                    Nome = "Dinheiro",
                    Ativo = true,
                    DataCriacao = dataSeedPadrao,
                    Observacao = "Pagamento em dinheiro na retirada."
                },
                new FormaPagamento
                {
                    Id = 3,
                    Nome = "Cartao de Debito",
                    Ativo = true,
                    DataCriacao = dataSeedPadrao,
                    Observacao = "Pagamento em cartao de debito."
                },
                new FormaPagamento
                {
                    Id = 4,
                    Nome = "Cartao de Credito",
                    Ativo = true,
                    DataCriacao = dataSeedPadrao,
                    Observacao = "Pagamento em cartao de credito."
                });

            base.OnModelCreating(modelBuilder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<string>()
                .HaveColumnType("varchar")
                .HaveMaxLength(200);

            configurationBuilder.Properties<decimal>()
                .HaveColumnType("decimal(18,2)");

            configurationBuilder.Properties<decimal?>()
                .HaveColumnType("decimal(18,2)");
        }
    }
}

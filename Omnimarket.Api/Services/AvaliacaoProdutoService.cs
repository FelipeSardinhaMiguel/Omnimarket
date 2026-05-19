using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Produtos.Avaliacoes;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Services
{
    public class AvaliacaoProdutoService
    {
        private readonly DataContext _context;

        public AvaliacaoProdutoService(DataContext context)
        {
            _context = context;
        }

        public async Task<PageResult<ProdutoAvaliacaoLeituraDto>> ListarPorProdutoAsync(int produtoId, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = Math.Clamp(pageSize, 1, 50);

            var query = BaseQuery()
                .Where(a => a.ProdutoId == produtoId);

            var total = await query.CountAsync();
            var avaliacoes = await query
                .OrderByDescending(a => a.DtCriacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<ProdutoAvaliacaoLeituraDto>
            {
                Items = avaliacoes.Select(Mapear).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PageResult<ProdutoAvaliacaoLeituraDto>> ListarPorLojaAsync(string slugLoja, int page = 1, int pageSize = 10)
        {
            page = page < 1 ? 1 : page;
            pageSize = Math.Clamp(pageSize, 1, 50);

            var slug = slugLoja.Trim().ToLowerInvariant();

            var query = BaseQuery()
                .Where(a => a.Loja.Slug == slug);

            var total = await query.CountAsync();
            var avaliacoes = await query
                .OrderByDescending(a => a.DtCriacao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<ProdutoAvaliacaoLeituraDto>
            {
                Items = avaliacoes.Select(Mapear).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<ProdutoAvaliacaoLeituraDto> CriarAsync(int produtoId, int usuarioId, ProdutoAvaliacaoCriacaoDto dto)
        {
            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Loja)
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            if (produto == null)
                throw new InvalidOperationException("Produto nao encontrado.");

            var pedido = await _context.TBL_PEDIDO
                .Include(p => p.Itens)
                .FirstOrDefaultAsync(p => p.Id == dto.PedidoId && p.UsuarioId == usuarioId);

            if (pedido == null)
                throw new InvalidOperationException("Pedido nao encontrado para este usuario.");

            if (pedido.StatusPedidosId != StatusPedido.Entregue)
                throw new InvalidOperationException("Somente pedidos entregues podem ser avaliados.");

            if (!pedido.Itens.Any(i => i.ProdutoId == produtoId))
                throw new InvalidOperationException("O produto informado nao pertence ao pedido selecionado.");

            var jaExiste = await _context.TBL_AVALIACAO_PRODUTO.AnyAsync(a =>
                a.ProdutoId == produtoId &&
                a.PedidoId == dto.PedidoId &&
                a.UsuarioId == usuarioId);

            if (jaExiste)
                throw new InvalidOperationException("Voce ja avaliou este produto nesse pedido.");

            var avaliacao = new AvaliacaoProduto
            {
                ProdutoId = produto.Id,
                LojaId = produto.LojaId,
                PedidoId = pedido.Id,
                UsuarioId = usuarioId,
                NotaProduto = dto.NotaProduto,
                NotaLoja = dto.NotaLoja ?? dto.NotaProduto,
                Titulo = LimparOpcional(dto.Titulo),
                Comentario = LimparOpcional(dto.Comentario),
                CompraVerificada = true,
                RecomendaProduto = dto.RecomendaProduto,
                DtCriacao = DateTimeOffset.UtcNow
            };

            _context.TBL_AVALIACAO_PRODUTO.Add(avaliacao);
            await _context.SaveChangesAsync();
            await RecalcularAgregadosAsync(produto.Id, produto.LojaId);

            var avaliacaoCarregada = await BaseQuery()
                .SingleAsync(a => a.Id == avaliacao.Id);

            return Mapear(avaliacaoCarregada);
        }

        public async Task<ProdutoAvaliacaoLeituraDto?> AtualizarAsync(int produtoId, int avaliacaoId, int usuarioId, ProdutoAvaliacaoAtualizacaoDto dto)
        {
            var avaliacao = await _context.TBL_AVALIACAO_PRODUTO
                .Include(a => a.Produto)
                .Include(a => a.Loja)
                .FirstOrDefaultAsync(a => a.Id == avaliacaoId && a.ProdutoId == produtoId);

            if (avaliacao == null)
                return null;

            if (avaliacao.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException("Voce nao pode editar a avaliacao de outro usuario.");

            avaliacao.NotaProduto = dto.NotaProduto;
            avaliacao.NotaLoja = dto.NotaLoja ?? dto.NotaProduto;
            avaliacao.Titulo = LimparOpcional(dto.Titulo);
            avaliacao.Comentario = LimparOpcional(dto.Comentario);
            avaliacao.RecomendaProduto = dto.RecomendaProduto;
            avaliacao.DtAtualizacao = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
            await RecalcularAgregadosAsync(avaliacao.ProdutoId, avaliacao.LojaId);

            var avaliacaoCarregada = await BaseQuery()
                .SingleAsync(a => a.Id == avaliacao.Id);

            return Mapear(avaliacaoCarregada);
        }

        private IQueryable<AvaliacaoProduto> BaseQuery()
        {
            return _context.TBL_AVALIACAO_PRODUTO
                .AsNoTracking()
                .Include(a => a.Produto)
                    .ThenInclude(p => p.Loja)
                .Include(a => a.Usuario)
                .Include(a => a.Loja);
        }

        private async Task RecalcularAgregadosAsync(int produtoId, int lojaId)
        {
            var agregadosProduto = await _context.TBL_AVALIACAO_PRODUTO
                .Where(a => a.ProdutoId == produtoId)
                .GroupBy(a => a.ProdutoId)
                .Select(g => new
                {
                    Media = g.Average(a => (double)a.NotaProduto),
                    Total = g.Count()
                })
                .FirstOrDefaultAsync();

            var produto = await _context.TBL_PRODUTO.FirstAsync(p => p.Id == produtoId);
            produto.MediaAvaliacao = Math.Round(agregadosProduto?.Media ?? 0, 2);
            produto.TotalAvaliacoes = agregadosProduto?.Total ?? 0;

            var agregadosLoja = await _context.TBL_AVALIACAO_PRODUTO
                .Where(a => a.LojaId == lojaId)
                .GroupBy(a => a.LojaId)
                .Select(g => new
                {
                    Media = g.Average(a => (double)a.NotaLoja),
                    Total = g.Count()
                })
                .FirstOrDefaultAsync();

            var loja = await _context.TBL_LOJA.FirstAsync(l => l.Id == lojaId);
            loja.MediaAvaliacao = Math.Round(agregadosLoja?.Media ?? 0, 2);
            loja.TotalAvaliacoes = agregadosLoja?.Total ?? 0;
            loja.DtAtualizacao = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
        }

        private static ProdutoAvaliacaoLeituraDto Mapear(AvaliacaoProduto avaliacao)
        {
            return new ProdutoAvaliacaoLeituraDto
            {
                Id = avaliacao.Id,
                ProdutoId = avaliacao.ProdutoId,
                NomeProduto = avaliacao.Produto.Nome,
                LojaId = avaliacao.LojaId,
                NomeLoja = avaliacao.Loja.NomeFantasia,
                SlugLoja = avaliacao.Loja.Slug,
                PedidoId = avaliacao.PedidoId,
                UsuarioId = avaliacao.UsuarioId,
                NomeComprador = FormatarNomeComprador(avaliacao.Usuario),
                NotaProduto = avaliacao.NotaProduto,
                NotaLoja = avaliacao.NotaLoja,
                Titulo = avaliacao.Titulo,
                Comentario = avaliacao.Comentario,
                CompraVerificada = avaliacao.CompraVerificada,
                RecomendaProduto = avaliacao.RecomendaProduto,
                DtCriacao = avaliacao.DtCriacao,
                DtAtualizacao = avaliacao.DtAtualizacao
            };
        }

        private static string FormatarNomeComprador(Usuario usuario)
        {
            var primeiroNome = (usuario.Nome ?? string.Empty).Trim();
            var sobrenome = (usuario.Sobrenome ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(primeiroNome))
                return "Cliente";

            if (string.IsNullOrWhiteSpace(sobrenome))
                return primeiroNome;

            return $"{primeiroNome} {char.ToUpperInvariant(sobrenome[0])}.";
        }

        private static string? LimparOpcional(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            return valor.Trim();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models.Dtos.Produtos;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Services.Interfaces;

namespace Omnimarket.Api.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly DataContext _context;

        public ProdutoService(DataContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProdutoLeituraDto>> GetAllAsync()
        {
            var produtos = await BaseQuery()
                .Where(p =>
                    p.Loja.Ativa &&
                    p.StatusPublicacao == StatusProduto.Publicado &&
                    p.Estoque > 0)
                .OrderByDescending(p => p.DtCriacao)
                .ThenBy(p => p.Nome)
                .ToListAsync();

            return produtos.Select(MapToDto).ToList();
        }

        public async Task<ProdutoLeituraDto?> GetByIdAsync(int id)
        {
            var produto = await BaseQuery()
                .FirstOrDefaultAsync(p =>
                    p.Id == id &&
                    p.Loja.Ativa &&
                    p.StatusPublicacao == StatusProduto.Publicado);

            return produto == null ? null : MapToDto(produto);
        }

        public async Task<ProdutoLeituraDto> CreateAsync(ProdutoCriacaoDto dto, int usuarioId)
        {
            var loja = await _context.TBL_LOJA
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);

            if (loja == null)
                throw new Exception("Crie uma loja antes de cadastrar produtos.");

            if (!loja.Ativa)
                throw new Exception("Sua loja precisa estar ativa para cadastrar produtos.");

            var sku = NormalizarSku(dto.Sku);

            if (await _context.TBL_PRODUTO.AnyAsync(p => p.LojaId == loja.Id && p.Sku == sku))
                throw new Exception("Ja existe um produto com esse SKU nesta loja.");

            var produto = new Produto
            {
                Nome = NormalizarTextoObrigatorio(dto.Nome, "Informe um nome valido para o produto."),
                Categoria = NormalizarTextoObrigatorio(dto.Categoria, "Informe uma categoria valida para o produto."),
                Sku = sku,
                Preco = dto.Preco,
                Estoque = dto.Estoque,
                Descricao = LimparOpcional(dto.Descricao),
                StatusPublicacao = dto.StatusPublicacao,
                LojaId = loja.Id,
                Loja = loja,
                DtCriacao = DateTimeOffset.UtcNow
            };

            AdicionarImagens(produto, dto.Imagens);

            _context.TBL_PRODUTO.Add(produto);
            await _context.SaveChangesAsync();

            return MapToDto(produto);
        }

        public async Task<bool> UpdateAsync(int id, ProdutoAtualizarDto dto, int usuarioId)
        {
            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Loja)
                .Include(p => p.Midias)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
                return false;

            if (produto.Loja.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException("Voce nao pode editar este produto.");

            var sku = NormalizarSku(dto.Sku);

            if (await _context.TBL_PRODUTO.AnyAsync(p => p.Id != id && p.LojaId == produto.LojaId && p.Sku == sku))
                throw new Exception("Ja existe um produto com esse SKU nesta loja.");

            produto.Nome = NormalizarTextoObrigatorio(dto.Nome, "Informe um nome valido para o produto.");
            produto.Categoria = NormalizarTextoObrigatorio(dto.Categoria, "Informe uma categoria valida para o produto.");
            produto.Sku = sku;
            produto.Preco = dto.Preco;
            produto.Estoque = dto.Estoque;
            produto.Descricao = LimparOpcional(dto.Descricao);
            produto.StatusPublicacao = dto.StatusPublicacao;
            produto.DtAtualizacao = DateTimeOffset.UtcNow;

            if (dto.Imagens != null)
                SincronizarImagens(produto, dto.Imagens);

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> AtualizarEstoqueAsync(int id, ProdutoAtualizarEstoqueDto dto, int usuarioId)
        {
            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Loja)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
                return false;

            if (produto.Loja.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException("Voce nao pode alterar o estoque deste produto.");

            if (produto.StatusPublicacao == StatusProduto.Desativado)
                throw new Exception("Produto desativado nao pode ter estoque alterado.");

            produto.Estoque = dto.Estoque;
            produto.DtAtualizacao = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(int id, int usuarioId)
        {
            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Loja)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (produto == null)
                return false;

            if (produto.Loja.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException("Voce nao pode excluir este produto.");

            produto.StatusPublicacao = StatusProduto.Desativado;
            produto.DtAtualizacao = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PageResult<ProdutoLeituraDto>> GetPagedAsync(ProdutoFiltroDto filtro)
        {
            var page = filtro.Page < 1 ? 1 : filtro.Page;
            var pageSize = filtro.PageSize < 1 ? 10 : filtro.PageSize;
            pageSize = pageSize > 50 ? 50 : pageSize;

            var query = BaseQuery()
                .Where(p => p.Loja.Ativa);

            if (filtro.StatusPublicacao.HasValue)
                query = query.Where(p => p.StatusPublicacao == filtro.StatusPublicacao.Value);
            else
                query = query.Where(p => p.StatusPublicacao == StatusProduto.Publicado);

            if (!string.IsNullOrWhiteSpace(filtro.Nome))
            {
                var like = $"%{filtro.Nome.Trim()}%";
                query = query.Where(p => EF.Functions.Like(p.Nome, like));
            }

            if (!string.IsNullOrWhiteSpace(filtro.Categoria))
            {
                var like = $"%{filtro.Categoria.Trim()}%";
                query = query.Where(p => EF.Functions.Like(p.Categoria, like));
            }

            if (filtro.LojaId.HasValue)
                query = query.Where(p => p.LojaId == filtro.LojaId.Value);

            if (filtro.PrecoMinimo.HasValue)
                query = query.Where(p => p.Preco >= filtro.PrecoMinimo.Value);

            if (filtro.PrecoMaximo.HasValue)
                query = query.Where(p => p.Preco <= filtro.PrecoMaximo.Value);

            if (filtro.Disponivel.HasValue)
            {
                if (filtro.Disponivel.Value)
                    query = query.Where(p => p.StatusPublicacao == StatusProduto.Publicado && p.Estoque > 0);
                else
                    query = query.Where(p => p.StatusPublicacao != StatusProduto.Publicado || p.Estoque <= 0);
            }

            var total = await query.CountAsync();
            var produtos = await query
                .OrderByDescending(p => p.DtCriacao)
                .ThenBy(p => p.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<ProdutoLeituraDto>
            {
                Items = produtos.Select(MapToDto).ToList(),
                Total = total,
                Page = page,
                PageSize = pageSize
            };
        }

        private IQueryable<Produto> BaseQuery()
        {
            return _context.TBL_PRODUTO
                .Include(p => p.Midias)
                .Include(p => p.Loja)
                .AsQueryable();
        }

        private void SincronizarImagens(Produto produto, IEnumerable<string> imagens)
        {
            var urls = NormalizarImagens(imagens).ToList();
            var fotosAtuais = produto.Midias
                .Where(m => m.Tipo == TipoMidiaProduto.Foto)
                .ToList();

            if (fotosAtuais.Count > 0)
            {
                _context.ProdutoMidia.RemoveRange(fotosAtuais);
                foreach (var foto in fotosAtuais)
                    produto.Midias.Remove(foto);
            }

            var midiasNaoFoto = produto.Midias
                .Where(m => m.Tipo != TipoMidiaProduto.Foto)
                .OrderBy(m => m.Ordem)
                .ToList();

            for (var index = 0; index < midiasNaoFoto.Count; index++)
                midiasNaoFoto[index].Ordem = urls.Count + index;

            AdicionarImagens(produto, urls);
        }

        private static void AdicionarImagens(Produto produto, IEnumerable<string>? imagens)
        {
            var urls = NormalizarImagens(imagens).ToList();
            if (urls.Count == 0)
                return;

            var ordemInicial = produto.Midias.Any() ? produto.Midias.Max(m => m.Ordem) + 1 : 0;

            for (var index = 0; index < urls.Count; index++)
            {
                produto.Midias.Add(new ProdutoMidia
                {
                    Tipo = TipoMidiaProduto.Foto,
                    Url = urls[index],
                    Ordem = ordemInicial + index
                });
            }
        }

        private static IEnumerable<string> NormalizarImagens(IEnumerable<string>? imagens)
        {
            if (imagens == null)
                return Array.Empty<string>();

            return imagens
                .Select(url => url?.Trim())
                .Where(url => !string.IsNullOrWhiteSpace(url))
                .Cast<string>()
                .ToList();
        }

        private static string NormalizarSku(string sku)
        {
            if (string.IsNullOrWhiteSpace(sku))
                throw new Exception("Informe um SKU valido para o produto.");

            return sku.Trim().ToUpperInvariant();
        }

        private static string NormalizarTextoObrigatorio(string? valor, string mensagemErro)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new Exception(mensagemErro);

            return valor.Trim();
        }

        private static string? LimparOpcional(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            return valor.Trim();
        }

        private static ProdutoLeituraDto MapToDto(Produto produto)
        {
            return new ProdutoLeituraDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Categoria = produto.Categoria,
                Sku = produto.Sku,
                Preco = produto.Preco,
                Estoque = produto.Estoque,
                Disponivel = produto.Disponivel,
                StatusPublicacao = produto.StatusPublicacao,
                Descricao = produto.Descricao,
                MediaAvaliacao = produto.MediaAvaliacao,
                TotalAvaliacoes = produto.TotalAvaliacoes,
                DtCriacao = produto.DtCriacao,
                DtAtualizacao = produto.DtAtualizacao,
                LojaId = produto.LojaId,
                NomeLoja = produto.Loja != null ? produto.Loja.NomeFantasia : string.Empty,
                SlugLoja = produto.Loja != null ? produto.Loja.Slug : string.Empty,
                Imagens = produto.Midias
                    .OrderBy(m => m.Ordem)
                    .Select(m => m.Url)
                    .ToList()
            };
        }
    }
}

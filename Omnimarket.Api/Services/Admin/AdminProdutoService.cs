using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Extensions;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Admin;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Services
{
    public class AdminProdutoService
    {
        private readonly DataContext _context;

        public AdminProdutoService(DataContext context)
        {
            _context = context;
        }

        public async Task<PageResult<AdminProdutoDto>> ListarProdutosAsync(
            string? busca,
            StatusProduto? status,
            int? lojaId,
            int page,
            int pageSize)
        {
            var query = _context.TBL_PRODUTO
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(busca))
            {
                var termo = $"%{busca.Trim()}%";
                query = query.Where(p =>
                    EF.Functions.Like(p.Nome, termo) ||
                    EF.Functions.Like(p.Categoria, termo) ||
                    EF.Functions.Like(p.Sku, termo) ||
                    EF.Functions.Like(p.Loja.Usuario.Nome, termo) ||
                    EF.Functions.Like(p.Loja.NomeFantasia, termo));
            }

            if (status.HasValue)
                query = query.Where(p => p.StatusPublicacao == status.Value);

            if (lojaId.HasValue)
                query = query.Where(p => p.LojaId == lojaId.Value);

            return await query
                .OrderByDescending(p => p.DtCriacao)
                .Select(p => new AdminProdutoDto
                {
                    Id = p.Id,
                    Nome = p.Nome,
                    Categoria = p.Categoria,
                    Sku = p.Sku,
                    Preco = p.Preco,
                    Estoque = p.Estoque,
                    Disponivel = p.StatusPublicacao == StatusProduto.Publicado && p.Estoque > 0,
                    StatusPublicacao = p.StatusPublicacao,
                    UsuarioId = p.Loja.UsuarioId,
                    NomeVendedor = p.Loja.Usuario.Nome + " " + p.Loja.Usuario.Sobrenome,
                    LojaId = p.LojaId,
                    NomeLoja = p.Loja.NomeFantasia,
                    DtCriacao = p.DtCriacao,
                    DtAtualizacao = p.DtAtualizacao
                })
                .ToPagedResultAsync(page, pageSize);
        }

        public async Task<AdminProdutoDto?> AtualizarStatusProdutoAsync(int produtoId, AdminAtualizarStatusProdutoDto dto)
        {
            if (!Enum.IsDefined(typeof(StatusProduto), dto.StatusPublicacao))
                throw new InvalidOperationException("Status de produto invalido.");

            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Loja)
                .ThenInclude(l => l.Usuario)
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            if (produto == null)
                return null;

            produto.StatusPublicacao = dto.StatusPublicacao;
            produto.DtAtualizacao = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();

            return produto.ToAdminDto();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Omni.Models.Entidades;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models.Dtos.Pedidos.Carrinho;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Services
{
    public class CarrinhoService
    {
        private readonly DataContext _context;

        public CarrinhoService(DataContext context)
        {
            _context = context;
        }

        public async Task<CarrinhoLeituraDto> ObterCarrinhoAsync(int usuarioId)
        {
            var carrinho = await ObterCarrinhoCompletoAsync(usuarioId);
            return MapearCarrinho(carrinho);
        }

        public async Task<CarrinhoLeituraDto> AdicionarItemAsync(int usuarioId, CarrinhoAdicionarDto dto)
        {
            if (dto.Quantidade <= 0)
                throw new InvalidOperationException("Quantidade deve ser maior que zero.");

            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Loja)
                .FirstOrDefaultAsync(p => p.Id == dto.ProdutoId);

            if (produto == null)
                throw new KeyNotFoundException("Produto nao encontrado.");

            var indisponivel = ValidarProdutoDisponivel(produto, usuarioId);
            if (indisponivel != null)
                throw new InvalidOperationException(indisponivel);

            var carrinho = await ObterOuCriarCarrinhoAsync(usuarioId);
            var item = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == dto.ProdutoId);
            var quantidadeFinal = (item?.Quantidade ?? 0) + dto.Quantidade;

            if (quantidadeFinal > produto.Estoque)
                throw new InvalidOperationException("Quantidade solicitada excede o estoque disponivel.");

            if (item != null)
            {
                item.Quantidade = quantidadeFinal;
            }
            else
            {
                carrinho.Itens.Add(new ItemCarrinho
                {
                    ProdutoId = dto.ProdutoId,
                    Quantidade = dto.Quantidade
                });
            }

            await _context.SaveChangesAsync();
            return await ObterCarrinhoAsync(usuarioId);
        }

        public async Task<CarrinhoLeituraDto> AtualizarQuantidadeAsync(
            int usuarioId,
            int produtoId,
            CarrinhoAtualizarQuantidadeDto dto)
        {
            if (dto.Quantidade <= 0)
                throw new InvalidOperationException("Quantidade deve ser maior que zero.");

            var carrinho = await _context.TBL_CARRINHO
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null)
                throw new KeyNotFoundException("Carrinho nao encontrado.");

            var item = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item == null)
                throw new KeyNotFoundException("Item nao encontrado no carrinho.");

            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Loja)
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            if (produto == null)
                throw new KeyNotFoundException("Produto nao encontrado.");

            var indisponivel = ValidarProdutoDisponivel(produto, usuarioId);
            if (indisponivel != null)
                throw new InvalidOperationException(indisponivel);

            if (dto.Quantidade > produto.Estoque)
                throw new InvalidOperationException("Quantidade solicitada excede o estoque disponivel.");

            item.Quantidade = dto.Quantidade;
            await _context.SaveChangesAsync();

            return await ObterCarrinhoAsync(usuarioId);
        }

        public async Task<CarrinhoLeituraDto> RemoverItemAsync(int usuarioId, int produtoId)
        {
            var carrinho = await _context.TBL_CARRINHO
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null)
                throw new KeyNotFoundException("Carrinho nao encontrado.");

            var item = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
            if (item == null)
                throw new KeyNotFoundException("Item nao encontrado no carrinho.");

            carrinho.Itens.Remove(item);
            await _context.SaveChangesAsync();

            return await ObterCarrinhoAsync(usuarioId);
        }

        public async Task<bool> LimparCarrinhoAsync(int usuarioId)
        {
            var carrinho = await _context.TBL_CARRINHO
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null || carrinho.Itens.Count == 0)
                return false;

            _context.TBL_ITEM_CARRINHO.RemoveRange(carrinho.Itens);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<Carrinho> ObterOuCriarCarrinhoAsync(int usuarioId)
        {
            var carrinho = await _context.TBL_CARRINHO
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho != null)
                return carrinho;

            carrinho = new Carrinho
            {
                UsuarioId = usuarioId
            };

            await _context.TBL_CARRINHO.AddAsync(carrinho);
            return carrinho;
        }

        private async Task<Carrinho?> ObterCarrinhoCompletoAsync(int usuarioId)
        {
            return await _context.TBL_CARRINHO
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .ThenInclude(p => p.Midias)
                .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
                .ThenInclude(p => p.Loja)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);
        }

        private static CarrinhoLeituraDto MapearCarrinho(Carrinho? carrinho)
        {
            if (carrinho == null)
                return new CarrinhoLeituraDto();

            var itens = carrinho.Itens
                .Select(i => new CarrinhoItemLeituraDto
                {
                    Id = i.Id,
                    ProdutoId = i.ProdutoId,
                    Nome = i.Produto.Nome,
                    Sku = i.Produto.Sku,
                    Categoria = i.Produto.Categoria,
                    LojaId = i.Produto.LojaId,
                    NomeLoja = i.Produto.Loja.NomeFantasia,
                    SlugLoja = i.Produto.Loja.Slug,
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.Produto.Preco,
                    Subtotal = i.Produto.Preco * i.Quantidade,
                    EstoqueDisponivel = i.Produto.Estoque,
                    StatusPublicacao = i.Produto.StatusPublicacao,
                    DisponivelParaCompra =
                        i.Produto.Loja.Ativa &&
                        i.Produto.StatusPublicacao == StatusProduto.Publicado &&
                        i.Produto.Estoque >= i.Quantidade,
                    ImagemPrincipal = i.Produto.Midias
                        .OrderBy(m => m.Ordem)
                        .Select(m => m.Url)
                        .FirstOrDefault()
                })
                .ToList();

            return new CarrinhoLeituraDto
            {
                CarrinhoId = carrinho.Id,
                Itens = itens,
                TotalItens = itens.Sum(i => i.Quantidade),
                ValorTotal = itens.Sum(i => i.Subtotal)
            };
        }

        private static string? ValidarProdutoDisponivel(Produto produto, int usuarioId)
        {
            if (produto.Loja.UsuarioId == usuarioId)
                return "Voce nao pode comprar seu proprio produto.";

            if (!produto.Loja.Ativa)
                return "A loja deste produto esta inativa no momento.";

            if (!produto.Disponivel && produto.StatusPublicacao != StatusProduto.Publicado)
                return "Produto nao esta disponivel para compra.";

            if (produto.Estoque <= 0)
                return "Produto sem estoque no momento.";

            return null;
        }
    }
}

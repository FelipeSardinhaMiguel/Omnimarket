using Microsoft.EntityFrameworkCore;
using Omni.Models.Entidades;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Pedidos;
using Omnimarket.Api.Models.Dtos.Pedidos.ItemPedido;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class PedidoService
    {
        private readonly DataContext _context;
        private readonly FinanceiroService _financeiroService;

        public PedidoService(DataContext context, FinanceiroService financeiroService)
        {
            _context = context;
            _financeiroService = financeiroService;
        }

        public async Task<Pedido> CriarPedido(int usuarioId, PedidoDto dto)
        {
            var usuarioExiste = await _context.TBL_USUARIO.AnyAsync(u => u.Id == usuarioId);
            if (!usuarioExiste)
                throw new Exception("Usuario nao encontrado.");

            if (!EntregaHelper.TipoEntregaValido(dto.TipoEntregaId))
                throw new Exception("Tipo de entrega invalido.");

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var enderecoEntrega = await ResolverEnderecoEntrega(usuarioId, dto.EnderecoId);
                var itensOrigem = await ResolverItensDoPedidoAsync(usuarioId, dto);

                var itensAgrupados = itensOrigem
                    .GroupBy(i => i.ProdutoId)
                    .Select(g => new ItemAgrupadoPedido(g.Key, g.Sum(x => x.Quantidade)))
                    .ToList();

                if (itensAgrupados.Count == 0 || itensAgrupados.Any(i => i.Quantidade <= 0))
                    throw new Exception("Quantidade invalida em um ou mais itens do pedido.");

                var produtoIds = itensAgrupados
                    .Select(i => i.ProdutoId)
                    .Distinct()
                    .ToList();

                var produtos = await _context.TBL_PRODUTO
                    .Include(p => p.Loja)
                    .Where(p => produtoIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                var pedido = new Pedido
                {
                    UsuarioId = usuarioId,
                    TipoLogradouroEntrega = EnumExtensions.GetDisplayName(enderecoEntrega.TipoLogradouro),
                    NomeEnderecoEntrega = enderecoEntrega.NomeEndereco,
                    NumeroEntrega = enderecoEntrega.Numero,
                    ComplementoEntrega = enderecoEntrega.Complemento,
                    CepEntrega = enderecoEntrega.Cep,
                    CidadeEntrega = enderecoEntrega.Cidade,
                    UfEntrega = enderecoEntrega.Uf,
                    TipoEntregaId = dto.TipoEntregaId,
                    Observacao = (dto.Observacao ?? string.Empty).Trim(),
                    StatusPedidosId = StatusPedido.Pendente,
                    DataPedido = DateTime.UtcNow
                };

                foreach (var item in itensAgrupados)
                {
                    if (!produtos.TryGetValue(item.ProdutoId, out var produto))
                        throw new Exception($"Produto {item.ProdutoId} nao encontrado.");

                    if (!produto.Loja.Ativa)
                        throw new Exception($"A loja do produto {item.ProdutoId} esta inativa.");

                    if (produto.Loja.UsuarioId == usuarioId)
                        throw new Exception($"Voce nao pode comprar o proprio produto {item.ProdutoId}.");

                    if (produto.StatusPublicacao != StatusProduto.Publicado)
                        throw new Exception($"Produto {item.ProdutoId} nao esta publicado para venda.");

                    if (produto.Estoque < item.Quantidade)
                        throw new Exception($"Estoque insuficiente para o produto {item.ProdutoId}.");

                    produto.Estoque -= item.Quantidade;

                    pedido.Itens.Add(new ItensPedido
                    {
                        ProdutoId = produto.Id,
                        Quantidade = item.Quantidade,
                        PrecoUnitario = produto.Preco,
                        ValorTotal = item.Quantidade * produto.Preco,
                        NomeProdutoSnapshot = produto.Nome,
                        NomeLojaSnapshot = produto.Loja.NomeFantasia,
                        DocumentoLojaSnapshot = produto.Loja.DocumentoFiscal,
                        TipoDocumentoLojaSnapshot = produto.Loja.TipoDocumentoFiscal.ToString()
                    });
                }

                pedido.ValorTotalProdutos = pedido.Itens.Sum(i => i.ValorTotal);
                pedido.ValorFrete = 0m;
                pedido.ValorTotalPedido = pedido.ValorTotalProdutos;

                await RemoverItensCompradosDoCarrinhoAsync(usuarioId, itensAgrupados);
                await _context.TBL_PEDIDO.AddAsync(pedido);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return pedido;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException(
                    "O estoque foi atualizado durante a finalizacao do pedido. Revise o carrinho e tente novamente.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<PedidoLeituraDto?> BuscarPedido(int id, int usuarioId)
        {
            var pedido = await _context.TBL_PEDIDO
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .ThenInclude(produto => produto.Loja)
                .FirstOrDefaultAsync(p => p.Id == id && p.UsuarioId == usuarioId);

            return pedido == null ? null : MapearPedido(pedido);
        }

        public async Task<List<PedidoLeituraDto>> ListarPedidosUsuario(int usuarioId)
        {
            var pedidos = await _context.TBL_PEDIDO
                .Where(p => p.UsuarioId == usuarioId)
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .ThenInclude(produto => produto.Loja)
                .OrderByDescending(p => p.DataPedido)
                .ToListAsync();

            return pedidos.Select(MapearPedido).ToList();
        }

        public async Task<bool> CancelarPedido(int pedidoId, int usuarioId)
        {
            var pedido = await _context.TBL_PEDIDO
                .Include(p => p.Itens)
                .ThenInclude(i => i.Produto)
                .ThenInclude(produto => produto.Loja)
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (pedido == null)
                return false;

            if (pedido.UsuarioId != usuarioId)
                throw new Exception("Voce nao pode cancelar pedidos que nao sao seus.");

            if (pedido.StatusPedidosId == StatusPedido.Cancelado)
                throw new Exception("Este pedido ja esta cancelado.");

            if (pedido.StatusPedidosId == StatusPedido.Enviado)
            {
                throw new Exception(
                    "Pedido enviado nao pode ser cancelado pelo cliente. Nesse caso o ideal e abrir um atendimento para devolucao.");
            }

            if (pedido.StatusPedidosId == StatusPedido.Entregue)
            {
                throw new Exception(
                    "Pedido entregue nao pode ser cancelado. Nesse caso o ideal e seguir o fluxo de devolucao.");
            }

            if (pedido.StatusPedidosId != StatusPedido.Pendente &&
                pedido.StatusPedidosId != StatusPedido.Pago)
            {
                throw new Exception("Somente pedidos pendentes ou pagos podem ser cancelados pelo cliente.");
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                await CancelarPedidoInternoAsync(
                    pedido,
                    usuarioId,
                    "cancelamento-cliente");

                await transaction.CommitAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                await transaction.RollbackAsync();
                throw new InvalidOperationException(
                    "O estoque de um ou mais produtos foi alterado durante o cancelamento. Tente novamente.");
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<Pedido?> MarcarPedidoComoEnviadoAsync(int pedidoId)
        {
            var pedido = await _context.TBL_PEDIDO
                .FirstOrDefaultAsync(p => p.Id == pedidoId);

            if (pedido == null)
                return null;

            if (pedido.StatusPedidosId == StatusPedido.Cancelado)
                throw new InvalidOperationException("Pedido cancelado nao pode ser enviado.");

            if (pedido.StatusPedidosId == StatusPedido.Enviado)
                throw new InvalidOperationException("Pedido ja foi enviado.");

            if (pedido.StatusPedidosId == StatusPedido.Entregue)
                throw new InvalidOperationException("Pedido ja foi entregue.");

            if (pedido.StatusPedidosId != StatusPedido.Pago)
                throw new InvalidOperationException("Somente pedidos pagos podem seguir para envio.");

            pedido.StatusPedidosId = StatusPedido.Enviado;
            await AtualizarStatusVendasDoPedidoAsync(pedidoId, StatusVenda.Enviada);
            await _context.SaveChangesAsync();

            return pedido;
        }

        public async Task<Pedido?> ConfirmarEntregaPedidoAsync(int pedidoId, int usuarioId)
        {
            var pedido = await _context.TBL_PEDIDO
                .FirstOrDefaultAsync(p => p.Id == pedidoId && p.UsuarioId == usuarioId);

            if (pedido == null)
                return null;

            if (pedido.StatusPedidosId == StatusPedido.Cancelado)
                throw new InvalidOperationException("Pedido cancelado nao pode ser confirmado como entregue.");

            if (pedido.StatusPedidosId == StatusPedido.Entregue)
                throw new InvalidOperationException("Pedido ja foi entregue.");

            if (pedido.StatusPedidosId != StatusPedido.Enviado)
                throw new InvalidOperationException("Somente pedidos enviados podem ser confirmados como entregues.");

            pedido.StatusPedidosId = StatusPedido.Entregue;
            await AtualizarStatusVendasDoPedidoAsync(pedidoId, StatusVenda.Concluida);
            await _context.SaveChangesAsync();

            return pedido;
        }

        private async Task<List<ItemPedidoDto>> ResolverItensDoPedidoAsync(int usuarioId, PedidoDto dto)
        {
            if (dto.Itens != null && dto.Itens.Count > 0)
                return dto.Itens;

            var carrinho = await _context.TBL_CARRINHO
                .AsNoTracking()
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null || carrinho.Itens.Count == 0)
                throw new Exception("Nenhum item foi informado e o carrinho esta vazio.");

            return carrinho.Itens
                .Select(i => new ItemPedidoDto
                {
                    ProdutoId = i.ProdutoId,
                    Quantidade = i.Quantidade
                })
                .ToList();
        }

        private async Task RemoverItensCompradosDoCarrinhoAsync(
            int usuarioId,
            IReadOnlyCollection<ItemAgrupadoPedido> itensComprados)
        {
            var carrinho = await _context.TBL_CARRINHO
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

            if (carrinho == null)
                return;

            var itensPorProduto = itensComprados.ToDictionary(i => i.ProdutoId, i => i.Quantidade);

            var itensRemover = new List<ItemCarrinho>();

            foreach (var itemCarrinho in carrinho.Itens)
            {
                if (!itensPorProduto.TryGetValue(itemCarrinho.ProdutoId, out var quantidadeComprada))
                    continue;

                if (itemCarrinho.Quantidade <= quantidadeComprada)
                {
                    itensRemover.Add(itemCarrinho);
                    continue;
                }

                itemCarrinho.Quantidade -= quantidadeComprada;
            }

            if (itensRemover.Count > 0)
                _context.TBL_ITEM_CARRINHO.RemoveRange(itensRemover);
        }

        private async Task AtualizarStatusVendasDoPedidoAsync(int pedidoId, StatusVenda novoStatus)
        {
            var vendas = await _context.TBL_VENDA
                .Where(v => v.PedidoId == pedidoId)
                .ToListAsync();

            if (vendas.Count == 0)
                return;

            var agora = DateTime.UtcNow;

            foreach (var venda in vendas)
            {
                if (venda.StatusVenda != novoStatus)
                {
                    venda.StatusVenda = novoStatus;
                    venda.DataAtualizacao = agora;
                }
            }
        }

        private async Task CancelarPedidoInternoAsync(
            Pedido pedido,
            int usuarioResponsavelId,
            string origem)
        {
            foreach (var item in pedido.Itens)
            {
                if (item.Produto != null)
                    item.Produto.Estoque += item.Quantidade;
            }

            pedido.StatusPedidosId = StatusPedido.Cancelado;

            await _financeiroService.CancelarFluxoFinanceiroDoPedidoAsync(
                pedido.Id,
                usuarioResponsavelId,
                origem);

            await _context.SaveChangesAsync();
        }

        private async Task<Endereco> ResolverEnderecoEntrega(int usuarioId, int? enderecoId)
        {
            var query = _context.TBL_ENDERECO
                .AsNoTracking()
                .Where(e => e.UsuarioId == usuarioId && e.Ativo);

            if (enderecoId.HasValue && enderecoId.Value > 0)
            {
                var enderecoSelecionado = await query.FirstOrDefaultAsync(e => e.Id == enderecoId.Value);

                if (enderecoSelecionado == null)
                    throw new Exception("Endereco de entrega nao encontrado.");

                return enderecoSelecionado;
            }

            var enderecoPadrao = await query
                .OrderByDescending(e => e.IsPrincipal)
                .ThenBy(e => e.Id)
                .FirstOrDefaultAsync();

            if (enderecoPadrao == null)
                throw new Exception("Nenhum endereco de entrega foi encontrado para o usuario.");

            return enderecoPadrao;
        }

        private static PedidoLeituraDto MapearPedido(Pedido pedido)
        {
            return new PedidoLeituraDto
            {
                Id = pedido.Id,
                Status = pedido.StatusPedidosId,
                TipoEntrega = EntregaHelper.ObterNomeTipoEntrega(pedido.TipoEntregaId),
                ValorTotalProdutos = pedido.ValorTotalProdutos,
                ValorFrete = pedido.ValorFrete,
                ValorTotalPedido = pedido.ValorTotalPedido,
                DataPedido = pedido.DataPedido,
                Observacao = pedido.Observacao,
                TipoLogradouroEntrega = pedido.TipoLogradouroEntrega,
                NomeEnderecoEntrega = pedido.NomeEnderecoEntrega,
                NumeroEntrega = pedido.NumeroEntrega,
                ComplementoEntrega = pedido.ComplementoEntrega,
                CepEntrega = pedido.CepEntrega,
                CidadeEntrega = pedido.CidadeEntrega,
                UfEntrega = pedido.UfEntrega,
                Itens = pedido.Itens
                    .OrderBy(i => i.Id)
                    .Select(i => new ItemPedidoLeituraDto
                    {
                        Id = i.Id,
                        ProdutoId = i.ProdutoId,
                        NomeProduto = i.Produto?.Nome ?? string.Empty,
                        SkuProduto = i.Produto?.Sku ?? string.Empty,
                        LojaId = i.Produto?.LojaId ?? 0,
                        NomeLoja = i.Produto?.Loja?.NomeFantasia ?? string.Empty,
                        SlugLoja = i.Produto?.Loja?.Slug ?? string.Empty,
                        Quantidade = i.Quantidade,
                        PrecoUnitario = i.PrecoUnitario,
                        ValorTotal = i.ValorTotal
                    })
                    .ToList()
            };
        }

        private sealed record ItemAgrupadoPedido(int ProdutoId, int Quantidade);
    }
}

using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models.Dtos.Pedidos.Recibos;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Utils;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Omnimarket.Api.Services
{
    public class ReciboPedidoService
    {
        private readonly DataContext _context;

        public ReciboPedidoService(DataContext context)
        {
            _context = context;
        }

        public async Task<byte[]?> GerarReciboPedidoAsync(int pedidoId, int usuarioId)
        {
            var pedido = await _context.TBL_PEDIDO
                .AsNoTracking()
                .Include(p => p.Usuario)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                        .ThenInclude(p => p.Loja)
                .FirstOrDefaultAsync(p => p.Id == pedidoId && p.UsuarioId == usuarioId);

            if (pedido == null)
                return null;

            ValidarStatusPedidoParaRecibo(pedido.StatusPedidosId);
            return GerarPdf(await MapearPedidoParaRecibo(pedido));
        }

        public async Task<byte[]?> GerarReciboPedidoParaVendedorAsync(int pedidoId, int vendedorId)
        {
            var loja = await _context.TBL_LOJA
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.UsuarioId == vendedorId);

            if (loja == null)
                return null;

            var pedido = await _context.TBL_PEDIDO
                .AsNoTracking()
                .Include(p => p.Usuario)
                .Include(p => p.Itens)
                    .ThenInclude(i => i.Produto)
                        .ThenInclude(p => p.Loja)
                .FirstOrDefaultAsync(p =>
                    p.Id == pedidoId &&
                    p.Itens.Any(i => i.Produto.LojaId == loja.Id));

            if (pedido == null)
                return null;

            ValidarStatusPedidoParaRecibo(pedido.StatusPedidosId);
            return GerarPdf(await MapearPedidoParaRecibo(pedido, loja.Id));
        }

        private async Task<ReciboPedidoDto> MapearPedidoParaRecibo(Pedido pedido, int? lojaIdFiltro = null)
        {
            var planoPagamento = await _context.TBL_PLANO_PAGAMENTO
                .AsNoTracking()
                .Include(p => p.FormaPagamento)
                .FirstOrDefaultAsync(p => p.PedidoId == pedido.Id);

            var itens = pedido.Itens.AsEnumerable();

            if (lojaIdFiltro.HasValue)
                itens = itens.Where(i => i.Produto.LojaId == lojaIdFiltro.Value);

            var itensRecibo = itens
                .Select(i => new ReciboPedidoItemDto
                {
                    NomeProduto = !string.IsNullOrWhiteSpace(i.NomeProdutoSnapshot)
                        ? i.NomeProdutoSnapshot
                        : i.Produto.Nome,
                    NomeLoja = !string.IsNullOrWhiteSpace(i.NomeLojaSnapshot)
                        ? i.NomeLojaSnapshot
                        : i.Produto.Loja.NomeFantasia,
                    DocumentoLoja = !string.IsNullOrWhiteSpace(i.DocumentoLojaSnapshot)
                        ? DocumentoFiscalSnapshotFormatter.Formatar(
                            i.TipoDocumentoLojaSnapshot,
                            i.DocumentoLojaSnapshot)
                        : DocumentoFiscalFormatter.Formatar(
                            i.Produto.Loja.TipoDocumentoFiscal,
                            i.Produto.Loja.DocumentoFiscal),
                    Quantidade = i.Quantidade,
                    PrecoUnitario = i.PrecoUnitario,
                    Subtotal = i.ValorTotal
                })
                .ToList();

            var valorProdutos = lojaIdFiltro.HasValue
                ? itensRecibo.Sum(i => i.Subtotal)
                : pedido.ValorTotalProdutos;

            var valorFrete = lojaIdFiltro.HasValue ? 0m : pedido.ValorFrete;

            return new ReciboPedidoDto
            {
                PedidoId = pedido.Id,
                DataPedido = pedido.DataPedido,
                TipoRecibo = lojaIdFiltro.HasValue ? "Vendedor" : "Comprador",
                StatusPedido = pedido.StatusPedidosId.ToString(),
                NomeComprador = $"{pedido.Usuario.Nome} {pedido.Usuario.Sobrenome}".Trim(),
                EmailComprador = pedido.Usuario.Email,
                EnderecoEntrega =
                    $"{pedido.TipoLogradouroEntrega} {pedido.NomeEnderecoEntrega}, {pedido.NumeroEntrega}" +
                    $"{(string.IsNullOrWhiteSpace(pedido.ComplementoEntrega) ? string.Empty : " - " + pedido.ComplementoEntrega)}, " +
                    $"{pedido.CidadeEntrega}/{pedido.UfEntrega} - CEP {pedido.CepEntrega}",
                ValorProdutos = valorProdutos,
                ValorFrete = valorFrete,
                ValorTotal = valorProdutos + valorFrete,
                FormaPagamento = planoPagamento?.FormaPagamento?.Nome ?? "Pagamento fake",
                StatusPagamento = planoPagamento?.StatusPagamento.ToString() ?? "Aprovado",
                Itens = itensRecibo
            };
        }

        private static void ValidarStatusPedidoParaRecibo(StatusPedido statusPedido)
        {
            if (statusPedido != StatusPedido.Pago &&
                statusPedido != StatusPedido.Enviado &&
                statusPedido != StatusPedido.Entregue)
            {
                throw new InvalidOperationException("O recibo so pode ser gerado para pedidos pagos, enviados ou entregues.");
            }
        }

        private static byte[] GerarPdf(ReciboPedidoDto recibo)
        {
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(35);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(10));

                    page.Header().Column(col =>
                    {
                        col.Item().Text("OmniMarket")
                            .FontSize(22)
                            .Bold();

                        col.Item().Text($"Comprovante de Venda - {recibo.TipoRecibo}")
                            .FontSize(15)
                            .SemiBold();

                        col.Item().Text("Este documento nao substitui nota fiscal.")
                            .FontSize(9)
                            .Italic();

                        col.Item().PaddingTop(10).LineHorizontal(1);
                    });

                    page.Content().PaddingVertical(20).Column(col =>
                    {
                        col.Spacing(12);

                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text($"Pedido no {recibo.PedidoId}").Bold();
                                c.Item().Text($"Data: {recibo.DataPedido:dd/MM/yyyy HH:mm}");
                                c.Item().Text($"Status do pedido: {recibo.StatusPedido}");
                            });

                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("Comprador").Bold();
                                c.Item().Text(recibo.NomeComprador);
                                c.Item().Text(recibo.EmailComprador);
                            });
                        });

                        col.Item().Text("Endereco de entrega").Bold();
                        col.Item().Text(recibo.EnderecoEntrega);

                        col.Item().Text("Itens do pedido").Bold();

                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.ConstantColumn(45);
                                columns.ConstantColumn(70);
                                columns.ConstantColumn(70);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellHeader).Text("Produto");
                                header.Cell().Element(CellHeader).Text("Loja");
                                header.Cell().Element(CellHeader).AlignCenter().Text("Qtd");
                                header.Cell().Element(CellHeader).AlignRight().Text("Unitario");
                                header.Cell().Element(CellHeader).AlignRight().Text("Subtotal");
                            });

                            foreach (var item in recibo.Itens)
                            {
                                table.Cell().Element(CellBody).Text(item.NomeProduto);
                                table.Cell().Element(CellBody).Column(c =>
                                {
                                    c.Item().Text(item.NomeLoja);
                                    c.Item().Text(item.DocumentoLoja).FontSize(8);
                                });
                                table.Cell().Element(CellBody).AlignCenter().Text(item.Quantidade.ToString());
                                table.Cell().Element(CellBody).AlignRight().Text($"R$ {item.PrecoUnitario:N2}");
                                table.Cell().Element(CellBody).AlignRight().Text($"R$ {item.Subtotal:N2}");
                            }
                        });

                        col.Item().AlignRight().Column(c =>
                        {
                            c.Item().Text($"Produtos: R$ {recibo.ValorProdutos:N2}");
                            c.Item().Text($"Frete: R$ {recibo.ValorFrete:N2}");
                            c.Item().Text($"Total: R$ {recibo.ValorTotal:N2}")
                                .FontSize(14)
                                .Bold();
                        });

                        col.Item().Text("Pagamento").Bold();
                        col.Item().Text($"Forma: {recibo.FormaPagamento}");
                        col.Item().Text($"Status: {recibo.StatusPagamento}");

                        col.Item().PaddingTop(10).Text(
                            "Recibo gerado automaticamente pelo OmniMarket para fins de comprovacao da venda. " +
                            "Este documento nao possui validade fiscal e nao substitui nota fiscal.")
                            .FontSize(9)
                            .Italic();
                    });

                    page.Footer().AlignCenter().Text(text =>
                    {
                        text.Span("OmniMarket - Recibo gerado em ");
                        text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
                });
            }).GeneratePdf();

            static IContainer CellHeader(IContainer container)
            {
                return container
                    .DefaultTextStyle(x => x.Bold())
                    .Padding(5)
                    .BorderBottom(1);
            }

            static IContainer CellBody(IContainer container)
            {
                return container
                    .Padding(5)
                    .BorderBottom(0.5f)
                    .BorderColor(Colors.Grey.Lighten2);
            }
        }
    }
}

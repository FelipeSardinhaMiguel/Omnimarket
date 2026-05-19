using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Produtos.Lojas;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class LojaService
    {
        private readonly DataContext _context;

        public LojaService(DataContext context)
        {
            _context = context;
        }

        // Retorna a loja vinculada ao usuario autenticado.
        public async Task<LojaGestaoLeituraDto?> ObterMinhaLojaAsync(int usuarioId)
        {
            var loja = await _context.TBL_LOJA
                .AsNoTracking()
                .Include(l => l.Endereco)
                .Include(l => l.Telefone)
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);

            return loja == null ? null : MapearGestao(loja);
        }

        public async Task<LojaMetricasResumoDto?> ObterMinhasMetricasAsync(int usuarioId)
        {
            var loja = await _context.TBL_LOJA
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);

            if (loja == null)
                return null;

            var pedidosDaLoja = _context.TBL_PEDIDO
                .AsNoTracking()
                .Where(p => p.Itens.Any(i => i.Produto.LojaId == loja.Id));

            var pedidosPorStatusBruto = await pedidosDaLoja
                .GroupBy(p => p.StatusPedidosId)
                .Select(g => new
                {
                    Status = g.Key,
                    Total = g.Count()
                })
                .ToListAsync();

            var pedidosPorStatusLookup = pedidosPorStatusBruto.ToDictionary(x => x.Status, x => x.Total);
            var totalPedidos = pedidosPorStatusBruto.Sum(x => x.Total);
            var pedidosCancelados = pedidosPorStatusLookup.TryGetValue(StatusPedido.Cancelado, out var totalCancelados)
                ? totalCancelados
                : 0;

            var vendasComReceita = new[]
            {
                StatusVenda.Paga,
                StatusVenda.Enviada,
                StatusVenda.Concluida
            };

            var resumoFaturamento = await _context.TBL_VENDA
                .AsNoTracking()
                .Where(v => v.VendedorId == usuarioId && vendasComReceita.Contains(v.StatusVenda))
                .GroupBy(_ => 1)
                .Select(g => new
                {
                    FaturamentoBruto = g.Sum(v => v.ValorBruto),
                    FaturamentoLiquido = g.Sum(v => v.ValorLiquido),
                    TicketMedio = g.Average(v => v.ValorBruto)
                })
                .FirstOrDefaultAsync();

            var pedidosComReceita = new[]
            {
                StatusPedido.Pago,
                StatusPedido.Enviado,
                StatusPedido.Entregue
            };

            var itensVendidosDaLoja = _context.TBL_ITENS_PEDIDO
                .AsNoTracking()
                .Where(i =>
                    i.Produto.LojaId == loja.Id &&
                    pedidosComReceita.Contains(i.Pedido.StatusPedidosId));

            var produtosMaisVendidosPorQuantidade = await itensVendidosDaLoja
                .GroupBy(i => new { i.ProdutoId, i.Produto.Nome, i.Produto.Sku })
                .Select(g => new LojaMetricaProdutoDto
                {
                    ProdutoId = g.Key.ProdutoId,
                    Nome = g.Key.Nome,
                    Sku = g.Key.Sku,
                    QuantidadeVendida = g.Sum(i => i.Quantidade),
                    ReceitaBruta = g.Sum(i => i.ValorTotal)
                })
                .OrderByDescending(p => p.QuantidadeVendida)
                .ThenByDescending(p => p.ReceitaBruta)
                .Take(5)
                .ToListAsync();

            var produtosMaisVendidosPorReceita = await itensVendidosDaLoja
                .GroupBy(i => new { i.ProdutoId, i.Produto.Nome, i.Produto.Sku })
                .Select(g => new LojaMetricaProdutoDto
                {
                    ProdutoId = g.Key.ProdutoId,
                    Nome = g.Key.Nome,
                    Sku = g.Key.Sku,
                    QuantidadeVendida = g.Sum(i => i.Quantidade),
                    ReceitaBruta = g.Sum(i => i.ValorTotal)
                })
                .OrderByDescending(p => p.ReceitaBruta)
                .ThenByDescending(p => p.QuantidadeVendida)
                .Take(5)
                .ToListAsync();

            return new LojaMetricasResumoDto
            {
                LojaId = loja.Id,
                NomeFantasia = loja.NomeFantasia,
                FaturamentoBruto = resumoFaturamento?.FaturamentoBruto ?? 0m,
                FaturamentoLiquido = resumoFaturamento?.FaturamentoLiquido ?? 0m,
                TicketMedio = Math.Round(resumoFaturamento?.TicketMedio ?? 0m, 2, MidpointRounding.AwayFromZero),
                TaxaCancelamento = totalPedidos == 0
                    ? 0
                    : Math.Round((double)pedidosCancelados / totalPedidos * 100, 2, MidpointRounding.AwayFromZero),
                MediaAvaliacao = loja.MediaAvaliacao,
                TotalAvaliacoes = loja.TotalAvaliacoes,
                PedidosPorStatus = Enum.GetValues(typeof(StatusPedido))
                    .Cast<StatusPedido>()
                    .Select(status => new LojaMetricaPedidoStatusDto
                    {
                        Status = status,
                        Total = pedidosPorStatusLookup.TryGetValue(status, out var total) ? total : 0
                    })
                    .ToList(),
                ProdutosMaisVendidosPorQuantidade = produtosMaisVendidosPorQuantidade,
                ProdutosMaisVendidosPorReceita = produtosMaisVendidosPorReceita
            };
        }

        // Retorna uma loja publica ativa pelo slug.
        public async Task<LojaLeituraDto?> ObterPorSlugAsync(string slug)
        {
            var slugNormalizado = NormalizarSlug(slug);
            if (string.IsNullOrWhiteSpace(slugNormalizado))
                return null;

            var loja = await _context.TBL_LOJA
                .AsNoTracking()
                .Include(l => l.Endereco)
                .Include(l => l.Telefone)
                .FirstOrDefaultAsync(l => l.Slug == slugNormalizado && l.Ativa);

            return loja == null ? null : MapearPublico(loja);
        }

        // Cria a loja do usuario. Cada usuario pode ter apenas uma loja.
        public async Task<LojaGestaoLeituraDto> CriarMinhaLojaAsync(int usuarioId, LojaCriacaoDto dto)
        {
            if (await _context.TBL_LOJA.AnyAsync(l => l.UsuarioId == usuarioId))
                throw new InvalidOperationException("Voce ja possui uma loja cadastrada.");

            var usuarioExiste = await _context.TBL_USUARIO.AnyAsync(u => u.Id == usuarioId);
            if (!usuarioExiste)
                throw new InvalidOperationException("Usuario nao encontrado.");

            var slugBase = string.IsNullOrWhiteSpace(dto.Slug) ? dto.NomeFantasia : dto.Slug;
            var documentoFiscal = ValidarENormalizarDocumentoFiscal(dto.TipoDocumentoFiscal, dto.DocumentoFiscal);

            var documentoJaCadastrado = await _context.TBL_LOJA.AnyAsync(l =>
                l.TipoDocumentoFiscal == dto.TipoDocumentoFiscal &&
                l.DocumentoFiscal == documentoFiscal);

            if (documentoJaCadastrado)
                throw new InvalidOperationException("Documento fiscal ja cadastrado.");

            var enderecoLoja = await CriarEnderecoLojaAsync(
                usuarioId,
                dto.UsarEnderecoUsuario,
                dto.EnderecoUsuarioId,
                dto.NovoEnderecoLoja,
                obrigatorio: true);

            var telefoneLoja = await CriarTelefoneLojaAsync(
                usuarioId,
                dto.UsarTelefoneUsuario,
                dto.TelefoneUsuarioId,
                dto.NovoTelefoneLoja,
                obrigatorio: true);

            var loja = new Loja
            {
                UsuarioId = usuarioId,
                NomeFantasia = dto.NomeFantasia.Trim(),
                Slug = await GerarSlugUnicoAsync(slugBase),
                TipoDocumentoFiscal = dto.TipoDocumentoFiscal,
                DocumentoFiscal = documentoFiscal,
                Descricao = LimparOpcional(dto.Descricao),
                EmailContato = LimparOpcional(dto.EmailContato)?.ToLowerInvariant(),
                Endereco = enderecoLoja,
                Telefone = telefoneLoja,
                Ativa = dto.Ativa,
                DtCriacao = DateTimeOffset.UtcNow
            };

            await _context.TBL_LOJA.AddAsync(loja);
            await _context.SaveChangesAsync();

            return MapearGestao(loja);
        }

        // Atualiza os dados da loja do proprio usuario.
        public async Task<LojaGestaoLeituraDto?> AtualizarMinhaLojaAsync(int usuarioId, LojaAtualizacaoDto dto)
        {
            var loja = await _context.TBL_LOJA
                .Include(l => l.Endereco)
                .Include(l => l.Telefone)
                .FirstOrDefaultAsync(l => l.UsuarioId == usuarioId);

            if (loja == null)
                return null;

            var slugBase = string.IsNullOrWhiteSpace(dto.Slug) ? dto.NomeFantasia : dto.Slug;
            var documentoFiscal = ValidarENormalizarDocumentoFiscal(dto.TipoDocumentoFiscal, dto.DocumentoFiscal);

            var documentoJaCadastrado = await _context.TBL_LOJA.AnyAsync(l =>
                l.Id != loja.Id &&
                l.TipoDocumentoFiscal == dto.TipoDocumentoFiscal &&
                l.DocumentoFiscal == documentoFiscal);

            if (documentoJaCadastrado)
                throw new InvalidOperationException("Documento fiscal ja cadastrado em outra loja.");

            loja.NomeFantasia = dto.NomeFantasia.Trim();
            loja.Slug = await GerarSlugUnicoAsync(slugBase, loja.Id);
            loja.TipoDocumentoFiscal = dto.TipoDocumentoFiscal;
            loja.DocumentoFiscal = documentoFiscal;
            loja.Descricao = LimparOpcional(dto.Descricao);
            loja.EmailContato = LimparOpcional(dto.EmailContato)?.ToLowerInvariant();
            loja.Ativa = dto.Ativa;
            loja.DtAtualizacao = DateTimeOffset.UtcNow;

            var novoEnderecoLoja = await CriarEnderecoLojaAsync(
                usuarioId,
                dto.UsarEnderecoUsuario,
                dto.EnderecoUsuarioId,
                dto.NovoEnderecoLoja,
                obrigatorio: false);

            if (novoEnderecoLoja != null)
                loja.Endereco = novoEnderecoLoja;

            var novoTelefoneLoja = await CriarTelefoneLojaAsync(
                usuarioId,
                dto.UsarTelefoneUsuario,
                dto.TelefoneUsuarioId,
                dto.NovoTelefoneLoja,
                obrigatorio: false);

            if (novoTelefoneLoja != null)
                loja.Telefone = novoTelefoneLoja;

            await _context.SaveChangesAsync();

            return MapearGestao(loja);
        }

        private async Task<Endereco?> CriarEnderecoLojaAsync(
            int usuarioId,
            bool usarEnderecoUsuario,
            int? enderecoUsuarioId,
            EnderecoCriacaoDto? novoEnderecoLoja,
            bool obrigatorio)
        {
            if (usarEnderecoUsuario)
            {
                if (novoEnderecoLoja != null)
                    throw new InvalidOperationException("Escolha entre usar um endereco do usuario ou informar um novo endereco da loja.");

                if (!enderecoUsuarioId.HasValue)
                    throw new InvalidOperationException("Informe EnderecoUsuarioId para usar um endereco ja cadastrado no usuario.");

                var enderecoUsuario = await _context.TBL_ENDERECO
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e =>
                        e.Id == enderecoUsuarioId.Value &&
                        e.UsuarioId == usuarioId &&
                        e.Ativo);

                if (enderecoUsuario == null)
                    throw new InvalidOperationException("Endereco do usuario nao encontrado.");

                var enderecoLoja = ClonarEndereco(enderecoUsuario, usuarioId);
                await _context.TBL_ENDERECO.AddAsync(enderecoLoja);

                return enderecoLoja;
            }

            if (enderecoUsuarioId.HasValue)
                throw new InvalidOperationException("Nao informe EnderecoUsuarioId quando UsarEnderecoUsuario for false.");

            if (novoEnderecoLoja == null)
            {
                if (obrigatorio)
                    throw new InvalidOperationException("Informe um novo endereco para a loja ou escolha um endereco ja cadastrado no usuario.");

                return null;
            }

            var endereco = CriarEnderecoLoja(usuarioId, novoEnderecoLoja);
            await _context.TBL_ENDERECO.AddAsync(endereco);

            return endereco;
        }

        private async Task<Telefone?> CriarTelefoneLojaAsync(
            int usuarioId,
            bool usarTelefoneUsuario,
            int? telefoneUsuarioId,
            TelefoneCriacaoDto? novoTelefoneLoja,
            bool obrigatorio)
        {
            if (usarTelefoneUsuario)
            {
                if (novoTelefoneLoja != null)
                    throw new InvalidOperationException("Escolha entre usar um telefone do usuario ou informar um novo telefone da loja.");

                if (!telefoneUsuarioId.HasValue)
                    throw new InvalidOperationException("Informe TelefoneUsuarioId para usar um telefone ja cadastrado no usuario.");

                var telefoneUsuario = await _context.TBL_TELEFONE
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t =>
                        t.Id == telefoneUsuarioId.Value &&
                        t.UsuarioId == usuarioId);

                if (telefoneUsuario == null)
                    throw new InvalidOperationException("Telefone do usuario nao encontrado.");

                var telefoneLoja = ClonarTelefone(telefoneUsuario, usuarioId);
                await _context.TBL_TELEFONE.AddAsync(telefoneLoja);

                return telefoneLoja;
            }

            if (telefoneUsuarioId.HasValue)
                throw new InvalidOperationException("Nao informe TelefoneUsuarioId quando UsarTelefoneUsuario for false.");

            if (novoTelefoneLoja == null)
            {
                if (obrigatorio)
                    throw new InvalidOperationException("Informe um novo telefone para a loja ou escolha um telefone ja cadastrado no usuario.");

                return null;
            }

            var telefone = CriarTelefoneLoja(usuarioId, novoTelefoneLoja);
            await _context.TBL_TELEFONE.AddAsync(telefone);

            return telefone;
        }

        // Gera slug unico com sufixo numerico quando necessario.
        private async Task<string> GerarSlugUnicoAsync(string valorBase, int? lojaIdIgnorar = null)
        {
            var slugBase = NormalizarSlug(valorBase);
            if (string.IsNullOrWhiteSpace(slugBase))
                throw new InvalidOperationException("Nao foi possivel gerar um slug valido para a loja.");

            var slug = slugBase;
            var contador = 2;

            while (await _context.TBL_LOJA.AnyAsync(l =>
                       l.Slug == slug &&
                       (!lojaIdIgnorar.HasValue || l.Id != lojaIdIgnorar.Value)))
            {
                slug = $"{slugBase}-{contador}";
                contador++;
            }

            return slug;
        }

        // Converte textos livres para um formato amigavel de URL.
        private static string NormalizarSlug(string texto)
        {
            if (string.IsNullOrWhiteSpace(texto))
                return string.Empty;

            var semAcento = texto.RemoverAcentos().ToLowerInvariant();
            semAcento = Regex.Replace(semAcento, @"[^a-z0-9\s-]", string.Empty);
            semAcento = Regex.Replace(semAcento, @"\s+", "-");
            semAcento = Regex.Replace(semAcento, @"-{2,}", "-");

            return semAcento.Trim('-');
        }

        private static string? LimparOpcional(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return null;

            return valor.Trim();
        }

        private static Endereco ClonarEndereco(Endereco origem, int usuarioId)
        {
            return new Endereco
            {
                UsuarioId = usuarioId,
                TipoLogradouro = origem.TipoLogradouro,
                NomeEndereco = origem.NomeEndereco,
                Numero = origem.Numero,
                Complemento = origem.Complemento,
                Cep = origem.Cep,
                Cidade = origem.Cidade,
                Uf = origem.Uf,
                IsPrincipal = false,
                Ativo = true
            };
        }

        private static Endereco CriarEnderecoLoja(int usuarioId, EnderecoCriacaoDto dto)
        {
            return new Endereco
            {
                UsuarioId = usuarioId,
                Cep = dto.Cep.Replace("-", string.Empty).Trim(),
                TipoLogradouro = dto.TipoLogradouro,
                NomeEndereco = dto.NomeEndereco.Trim(),
                Numero = dto.Numero.Trim(),
                Complemento = LimparOpcional(dto.Complemento),
                Cidade = dto.Cidade.Trim(),
                Uf = dto.Uf.Trim().ToUpperInvariant(),
                IsPrincipal = false,
                Ativo = true
            };
        }

        private static Telefone ClonarTelefone(Telefone origem, int usuarioId)
        {
            return new Telefone
            {
                UsuarioId = usuarioId,
                NumeroE164 = origem.NumeroE164,
                IsPrincipal = false
            };
        }

        private static Telefone CriarTelefoneLoja(int usuarioId, TelefoneCriacaoDto dto)
        {
            var resultado = ValidadorTelefone.ValidarCelularBr(dto.Ddd, dto.Numero);
            if (!resultado.Valido)
                throw new InvalidOperationException("Telefone invalido.");

            return new Telefone
            {
                UsuarioId = usuarioId,
                NumeroE164 = resultado.E164!,
                IsPrincipal = false
            };
        }

        private static string ValidarENormalizarDocumentoFiscal(
            TipoDocumentoFiscalLoja tipoDocumentoFiscal,
            string documentoFiscal)
        {
            var documento = new string((documentoFiscal ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());

            if (tipoDocumentoFiscal == TipoDocumentoFiscalLoja.CPF)
            {
                if (documento.Length != 11)
                    throw new InvalidOperationException("CPF deve conter 11 digitos.");

                if (!CpfValidador.ValidarCpf(documento))
                    throw new InvalidOperationException("CPF invalido.");

                return documento;
            }

            if (tipoDocumentoFiscal == TipoDocumentoFiscalLoja.CNPJ)
            {
                if (documento.Length != 14)
                    throw new InvalidOperationException("CNPJ deve conter 14 digitos.");

                if (!CnpjValidador.ValidarCnpj(documento))
                    throw new InvalidOperationException("CNPJ invalido.");

                return documento;
            }

            throw new InvalidOperationException("Tipo de documento fiscal invalido.");
        }

        private static LojaLeituraDto MapearPublico(Loja loja)
        {
            return new LojaLeituraDto
            {
                Id = loja.Id,
                UsuarioId = loja.UsuarioId,
                NomeFantasia = loja.NomeFantasia,
                Slug = loja.Slug,
                TipoDocumentoFiscal = loja.TipoDocumentoFiscal,
                DocumentoFiscalFormatado = DocumentoFiscalFormatter.Formatar(
                    loja.TipoDocumentoFiscal,
                    loja.DocumentoFiscal),
                Descricao = loja.Descricao,
                EmailContato = loja.EmailContato,
                EnderecoId = loja.EnderecoId,
                Cep = loja.Endereco?.Cep,
                Cidade = loja.Endereco?.Cidade,
                Uf = loja.Endereco?.Uf,
                NomeEndereco = loja.Endereco?.NomeEndereco,
                NumeroEndereco = loja.Endereco?.Numero,
                ComplementoEndereco = loja.Endereco?.Complemento,
                TelefoneId = loja.TelefoneId,
                NumeroTelefone = loja.Telefone?.NumeroE164,
                TipoTelefone = ObterTipoTelefone(loja.Telefone),
                Ativa = loja.Ativa,
                MediaAvaliacao = loja.MediaAvaliacao,
                TotalAvaliacoes = loja.TotalAvaliacoes,
                DtCriacao = loja.DtCriacao,
                DtAtualizacao = loja.DtAtualizacao
            };
        }

        private static LojaGestaoLeituraDto MapearGestao(Loja loja)
        {
            return new LojaGestaoLeituraDto
            {
                Id = loja.Id,
                UsuarioId = loja.UsuarioId,
                NomeFantasia = loja.NomeFantasia,
                Slug = loja.Slug,
                TipoDocumentoFiscal = loja.TipoDocumentoFiscal,
                DocumentoFiscal = loja.DocumentoFiscal,
                DocumentoFiscalFormatado = DocumentoFiscalFormatter.Formatar(
                    loja.TipoDocumentoFiscal,
                    loja.DocumentoFiscal),
                Descricao = loja.Descricao,
                EmailContato = loja.EmailContato,
                EnderecoId = loja.EnderecoId,
                Cep = loja.Endereco?.Cep,
                Cidade = loja.Endereco?.Cidade,
                Uf = loja.Endereco?.Uf,
                NomeEndereco = loja.Endereco?.NomeEndereco,
                NumeroEndereco = loja.Endereco?.Numero,
                ComplementoEndereco = loja.Endereco?.Complemento,
                TelefoneId = loja.TelefoneId,
                NumeroTelefone = loja.Telefone?.NumeroE164,
                TipoTelefone = ObterTipoTelefone(loja.Telefone),
                Ativa = loja.Ativa,
                MediaAvaliacao = loja.MediaAvaliacao,
                TotalAvaliacoes = loja.TotalAvaliacoes,
                DtCriacao = loja.DtCriacao,
                DtAtualizacao = loja.DtAtualizacao
            };
        }

        private static string? ObterTipoTelefone(Telefone? telefone)
        {
            return telefone == null ? null : TipoTelefoneBr.Celular.ToString();
        }
    }
}

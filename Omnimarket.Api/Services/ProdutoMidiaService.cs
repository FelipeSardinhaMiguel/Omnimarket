using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models.Dtos.Produtos.Midias;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Services
{
    public class ProdutoMidiaService
    {
        private const int QuantidadeMaximaArquivos = 5;
        private const long TamanhoMaximoArquivo = 5 * 1024 * 1024;

        private static readonly HashSet<string> ExtensoesPermitidas = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp"
        };

        private readonly DataContext _context;

        public ProdutoMidiaService(DataContext context)
        {
            _context = context;
        }

        public async Task<List<ProdutoMidiaLeituraDto>> ListarAsync(int produtoId)
        {
            var midias = await _context.ProdutoMidia
                .Where(m => m.ProdutoId == produtoId)
                .OrderBy(m => m.Ordem)
                .ToListAsync();

            return midias.Select(MapearMidia).ToList();
        }

        public async Task<List<ProdutoMidiaLeituraDto>> UploadMidiasAsync(
            int produtoId,
            int usuarioId,
            List<IFormFile> arquivos)
        {
            if (arquivos is null || arquivos.Count == 0)
                throw new InvalidOperationException("Envie ao menos 1 arquivo.");

            if (arquivos.Count > QuantidadeMaximaArquivos)
                throw new InvalidOperationException($"Envie no maximo {QuantidadeMaximaArquivos} arquivos por vez.");

            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Midias)
                .Include(p => p.Loja)
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            if (produto == null)
                throw new KeyNotFoundException("Produto nao encontrado.");

            if (produto.Loja.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException();

            var pasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", produtoId.ToString());
            Directory.CreateDirectory(pasta);

            var ordemAtual = produto.Midias.Any() ? produto.Midias.Max(m => m.Ordem) + 1 : 0;
            var novasMidias = new List<ProdutoMidia>();

            foreach (var arquivo in arquivos)
            {
                if (arquivo.Length == 0)
                    continue;

                if (arquivo.Length > TamanhoMaximoArquivo)
                    throw new InvalidOperationException($"O arquivo {arquivo.FileName} ultrapassa o limite de 5 MB.");

                var extensao = Path.GetExtension(arquivo.FileName).ToLowerInvariant();
                if (!ExtensoesPermitidas.Contains(extensao))
                    throw new InvalidOperationException($"A extensao {extensao} nao e permitida.");

                var nomeSeguro = $"{Guid.NewGuid():N}{extensao}";
                var caminho = Path.Combine(pasta, nomeSeguro);

                await using var stream = System.IO.File.Create(caminho);
                await arquivo.CopyToAsync(stream);

                novasMidias.Add(new ProdutoMidia
                {
                    ProdutoId = produtoId,
                    Tipo = TipoMidiaProduto.Foto,
                    Url = $"/uploads/{produtoId}/{nomeSeguro}",
                    ContentType = arquivo.ContentType,
                    Ordem = ordemAtual++
                });
            }

            if (novasMidias.Count == 0)
                throw new InvalidOperationException("Nenhum arquivo valido foi enviado.");

            await _context.ProdutoMidia.AddRangeAsync(novasMidias);
            await _context.SaveChangesAsync();

            return novasMidias
                .OrderBy(m => m.Ordem)
                .Select(MapearMidia)
                .ToList();
        }

        public async Task RemoverAsync(int produtoId, int midiaId, int usuarioId)
        {
            var produto = await _context.TBL_PRODUTO
                .Include(p => p.Loja)
                .FirstOrDefaultAsync(p => p.Id == produtoId);

            if (produto == null)
                throw new KeyNotFoundException("Produto nao encontrado.");

            if (produto.Loja.UsuarioId != usuarioId)
                throw new UnauthorizedAccessException();

            var midia = await _context.ProdutoMidia
                .FirstOrDefaultAsync(m => m.Id == midiaId && m.ProdutoId == produtoId);

            if (midia == null)
                throw new KeyNotFoundException("Midia nao encontrada.");

            if (midia.Url.StartsWith("/uploads/", StringComparison.OrdinalIgnoreCase))
            {
                var caminhoLocal = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    midia.Url.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (System.IO.File.Exists(caminhoLocal))
                    System.IO.File.Delete(caminhoLocal);
            }

            _context.ProdutoMidia.Remove(midia);
            await _context.SaveChangesAsync();
        }

        private static ProdutoMidiaLeituraDto MapearMidia(ProdutoMidia midia)
        {
            return new ProdutoMidiaLeituraDto
            {
                Id = midia.Id,
                Tipo = midia.Tipo,
                Url = midia.Url,
                ContentType = midia.ContentType,
                Ordem = midia.Ordem
            };
        }
    }
}

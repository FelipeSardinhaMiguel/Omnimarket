using Omnimarket.Api.Models.Dtos.Produtos;
using Omnimarket.Api.Models.Entidades;

namespace Omnimarket.Api.Services.Interfaces
{
    // Define o contrato usado pelo controller para consultar e manipular produtos.
    public interface IProdutoService
    {
        Task<IEnumerable<ProdutoLeituraDto>> GetAllAsync();
        Task<ProdutoLeituraDto?> GetByIdAsync(int id);
        Task<ProdutoLeituraDto> CreateAsync(ProdutoCriacaoDto dto, int usuarioId);
        Task<bool> UpdateAsync(int id, ProdutoAtualizarDto dto, int usuarioId);
        Task<bool> AtualizarEstoqueAsync(int id, ProdutoAtualizarEstoqueDto dto, int usuarioId);
        Task<bool> DeleteAsync(int id, int usuarioId);
        Task<PageResult<ProdutoLeituraDto>> GetPagedAsync(ProdutoFiltroDto filtro);
    }
}

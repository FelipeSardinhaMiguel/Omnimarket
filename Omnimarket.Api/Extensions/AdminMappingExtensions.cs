using Omnimarket.Api.Models.Dtos.Admin;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Entidades;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Extensions
{
    public static class AdminMappingExtensions
    {
        public static AdminUsuarioDto ToAdminDto(this Usuario usuario)
        {
            return new AdminUsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome + " " + usuario.Sobrenome,
                Email = usuario.Email,
                Cpf = usuario.Cpf,
                Role = usuario.Role,
                DataCadastro = usuario.DataCadastro,
                DataAcesso = usuario.DataAcesso,
                PossuiLoja = usuario.Loja != null,
                LojaAtiva = usuario.Loja != null && usuario.Loja.Ativa,
                TotalProdutos = usuario.Loja?.Produtos.Count ?? 0,
                TotalPedidos = usuario.Pedidos.Count
            };
        }

        public static AdminLojaDto ToAdminDto(this Loja loja)
        {
            return new AdminLojaDto
            {
                Id = loja.Id,
                UsuarioId = loja.UsuarioId,
                NomeFantasia = loja.NomeFantasia,
                Slug = loja.Slug,
                Cidade = loja.Endereco?.Cidade,
                Uf = loja.Endereco?.Uf,
                Ativa = loja.Ativa,
                NomeResponsavel = loja.Usuario.Nome + " " + loja.Usuario.Sobrenome,
                EmailResponsavel = loja.Usuario.Email,
                TotalProdutos = loja.Produtos.Count,
                ProdutosPublicados = loja.Produtos.Count(p => p.StatusPublicacao == StatusProduto.Publicado),
                DtCriacao = loja.DtCriacao,
                DtAtualizacao = loja.DtAtualizacao
            };
        }

        public static AdminProdutoDto ToAdminDto(this Produto produto)
        {
            return new AdminProdutoDto
            {
                Id = produto.Id,
                Nome = produto.Nome,
                Categoria = produto.Categoria,
                Sku = produto.Sku,
                Preco = produto.Preco,
                Estoque = produto.Estoque,
                Disponivel = produto.StatusPublicacao == StatusProduto.Publicado && produto.Estoque > 0,
                StatusPublicacao = produto.StatusPublicacao,
                UsuarioId = produto.Loja.UsuarioId,
                NomeVendedor = produto.Loja.Usuario.Nome + " " + produto.Loja.Usuario.Sobrenome,
                LojaId = produto.LojaId,
                NomeLoja = produto.Loja.NomeFantasia,
                DtCriacao = produto.DtCriacao,
                DtAtualizacao = produto.DtAtualizacao
            };
        }
    }
}

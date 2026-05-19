using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omnimarket.Api.Models.Entidades;

namespace Omni.Models.Entidades
{
    public class ItemCarrinho
    {
        public int Id { get; set; }

        public int CarrinhoId { get; set; }
        public Carrinho Carrinho { get; set; } = null!;

        public int ProdutoId { get; set; }
        public Produto Produto { get; set; } = null!;

        public int Quantidade { get; set; }
    }
}
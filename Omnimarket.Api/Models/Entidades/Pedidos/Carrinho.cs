using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omnimarket.Api.Models;

namespace Omni.Models.Entidades
{
    public class Carrinho
    {
        public int Id { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public List<ItemCarrinho> Itens { get; set; } = new();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Omnimarket.Api.Models.Enum;
using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Dtos.Produtos.Midias
{
    public class ProdutoMidiaCriacaoDto
    {
        [Required]
        public TipoMidiaProduto Tipo { get; set; }

        [Required, StringLength(500)]
        public string Url { get; set; } = string.Empty;

        [StringLength(100)]
        public string? ContentType { get; set; }

        public int Ordem { get; set; } = 0;
    }
}
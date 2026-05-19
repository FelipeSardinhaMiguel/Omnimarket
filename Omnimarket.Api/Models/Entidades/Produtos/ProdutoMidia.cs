using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models.Entidades
{
    public class ProdutoMidia
    {
        public int Id { get; set; }

        [Required]
        public int ProdutoId { get; set; }

        public Produto Produto { get; set; } = null!;

        [Required]
        public TipoMidiaProduto Tipo { get; set; }

        // Opção A (recomendada): link do arquivo no storage
        [Required, StringLength(500)]
        public string Url { get; set; } = string.Empty;

        // Metadados úteis
        [StringLength(100)]
        public string? ContentType { get; set; }   // image/jpeg, video/mp4 etc.

        public int Ordem { get; set; } = 0;        // ordenação no carrossel

        public DateTimeOffset DtCriacao { get; set; } = DateTimeOffset.UtcNow;
    }
}
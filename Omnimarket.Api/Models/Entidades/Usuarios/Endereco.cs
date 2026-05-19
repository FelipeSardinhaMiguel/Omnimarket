using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Models
{
    public class Endereco
    {
        [Key]
        public int Id { get; set; }

        // 🔗 Relacionamento com usuário
        [Required]
        public int UsuarioId { get; set; }

        [ForeignKey(nameof(UsuarioId))]
        public Usuario Usuario { get; set; } = null!;

        // 🏠 Tipo de logradouro (Enum)
        [Required]
        public TiposLogradouroBR TipoLogradouro { get; set; }

        // 📍 Nome da rua/avenida
        [Required]
        [StringLength(200)]
        public string NomeEndereco { get; set; } = string.Empty;

        // 🔢 Número
        [Required]
        [StringLength(20)]
        public string Numero { get; set; } = string.Empty;

        // ➕ Complemento
        [StringLength(80)]
        public string? Complemento { get; set; }

        // 📮 CEP (somente números recomendado)
        [Required]
        [StringLength(8, MinimumLength = 8)]
        public string Cep { get; set; } = string.Empty;

        // 🌆 Cidade
        [Required]
        [StringLength(120)]
        public string Cidade { get; set; } = string.Empty;

        // 🗺️ UF
        [Required]
        [StringLength(2, MinimumLength = 2)]
        public string Uf { get; set; } = string.Empty;

        // ⭐ Endereço principal
        public bool IsPrincipal { get; set; } = false;

        public bool Ativo { get; set; } = true;
    }
}

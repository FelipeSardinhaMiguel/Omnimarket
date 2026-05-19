using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Models.Entidades;

namespace Omnimarket.Api.Models
{
    [Index(nameof(Email), IsUnique = true)]
    [Index(nameof(Cpf), IsUnique = true)]
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(11)]
        public string Cpf { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Sobrenome { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();

        public byte[]? Foto { get; set; }

        public DateTime? DataAcesso { get; set; }

        public DateTime DataCadastro { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string PasswordString { get; set; } = string.Empty;

        [NotMapped]
        public string Token { get; set; } = string.Empty;

        public bool AceitouTermos { get; set; }
        public DateTime? DataAceiteTermos { get; set; }

        [StringLength(30)]
        public string Role { get; set; } = "User";

        public List<Telefone> Telefones { get; set; } = new();
        public List<Endereco> Enderecos { get; set; } = new();
        public List<Pedido> Pedidos { get; set; } = new();

        public Loja? Loja { get; set; }
    }
}

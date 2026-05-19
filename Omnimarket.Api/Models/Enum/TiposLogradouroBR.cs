using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Omnimarket.Api.Models.Enum
{
    [JsonConverter(typeof(JsonStringEnumConverter))] // 🔥 importante pro JSON
    public enum TiposLogradouroBR
    {
        Nenhum = 0,

        [Display(Name = "Rua (R)")]
        Rua = 1,

        [Display(Name = "Avenida (AV)")]
        Avenida = 2,

        [Display(Name = "Travessa (TV)")]
        Travessa = 3,

        [Display(Name = "Alameda (AL)")]
        Alameda = 4,

        [Display(Name = "Praça (PC)")]
        Praca = 5,

        [Display(Name = "Estrada (EST)")]
        Estrada = 6,

        [Display(Name = "Rodovia (ROD)")]
        Rodovia = 7,

        [Display(Name = "Viela (VLA)")]
        Viela = 8,

        [Display(Name = "Vila (VL)")]
        Vila = 9,

        [Display(Name = "Largo (LRG)")]
        Largo = 10,

        [Display(Name = "Ladeira (LD)")]
        Ladeira = 11,

        [Display(Name = "Conjunto (CJ)")]
        Conjunto = 12,

        [Display(Name = "Quadra (Q)")]
        Quadra = 13,

        [Display(Name = "Beco (BC)")]
        Beco = 14
    }
}
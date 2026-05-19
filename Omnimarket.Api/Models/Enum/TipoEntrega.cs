using System.ComponentModel.DataAnnotations;

namespace Omnimarket.Api.Models.Enum
{
    public enum TipoEntrega
    {
        [Display(Name = "Retirada")]
        Retirada = 1,

        [Display(Name = "Entrega local")]
        EntregaLocal = 2,

        [Display(Name = "Correios")]
        Correios = 3,

        [Display(Name = "Motoboy")]
        Motoboy = 4
    }
}

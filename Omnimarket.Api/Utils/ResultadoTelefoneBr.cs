using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Omnimarket.Api.Utils
{
     public enum TipoTelefoneBr
    {
        Fixo,
        Celular,
        FixoOuCelular
    }

    public sealed record ResultadoTelefoneBr(
        bool Valido,
        short? Ddd,
        TipoTelefoneBr? Tipo,
        string? E164
    );

}

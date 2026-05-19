using PhoneNumbers;
using Omnimarket.Api.Models.Dtos.Usuarios.Telefones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Omnimarket.Api.Utils;

public static class ValidadorTelefone
{
    public static ResultadoTelefoneBr ValidarCelularBr(string ddd, string numero)
        => ValidarCelularBr($"+55{SomenteDigitos(ddd)}{SomenteDigitos(numero)}");

     public static ResultadoTelefoneBr ValidarCelularBr(string telefone)
    {
        if (string.IsNullOrWhiteSpace(telefone))
            return new(false, null, null, null);

        var util = PhoneNumberUtil.GetInstance();

        PhoneNumber n;
        try
        {
            n = util.Parse(telefone, "BR");
        }
        catch (NumberParseException)
        {
            return new(false, null, null, null);
        }

        if (!util.IsValidNumberForRegion(n, "BR"))
            return new(false, null, null, null);

        var type = util.GetNumberType(n);
        if (type != PhoneNumberType.MOBILE)
            return new(false, null, null, null);

        var nsn = util.GetNationalSignificantNumber(n);
        var ndcLen = util.GetLengthOfNationalDestinationCode(n);
        if (ndcLen <= 0 || nsn.Length < ndcLen)
            return new(false, null, null, null);

        if (!short.TryParse(nsn.Substring(0, ndcLen), out var dddExtraido))
            return new(false, null, null, null);

        var e164 = util.Format(n, PhoneNumberFormat.E164);

        return new(true, dddExtraido, TipoTelefoneBr.Celular, e164);
    }

    private static string SomenteDigitos(string s)
        => new string((s ?? "").Where(char.IsDigit).ToArray());

}

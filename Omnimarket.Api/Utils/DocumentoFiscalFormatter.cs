using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Utils
{
    public static class DocumentoFiscalFormatter
    {
        public static string Formatar(TipoDocumentoFiscalLoja tipo, string? documento)
        {
            var numeros = new string((documento ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());

            if (tipo == TipoDocumentoFiscalLoja.CPF && numeros.Length == 11)
            {
                return Convert.ToUInt64(numeros).ToString(@"000\.000\.000\-00");
            }

            if (tipo == TipoDocumentoFiscalLoja.CNPJ && numeros.Length == 14)
            {
                return Convert.ToUInt64(numeros).ToString(@"00\.000\.000\/0000\-00");
            }

            return documento ?? string.Empty;
        }
    }
}

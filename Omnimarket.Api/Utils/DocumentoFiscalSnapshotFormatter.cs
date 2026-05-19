using Omnimarket.Api.Models.Enum;

namespace Omnimarket.Api.Utils
{
    public static class DocumentoFiscalSnapshotFormatter
    {
        public static string Formatar(string? tipoDocumento, string? documento)
        {
            var numeros = new string((documento ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());

            if (string.Equals(tipoDocumento, TipoDocumentoFiscalLoja.CPF.ToString(), StringComparison.OrdinalIgnoreCase)
                && numeros.Length == 11)
            {
                return Convert.ToUInt64(numeros).ToString(@"000\.000\.000\-00");
            }

            if (string.Equals(tipoDocumento, TipoDocumentoFiscalLoja.CNPJ.ToString(), StringComparison.OrdinalIgnoreCase)
                && numeros.Length == 14)
            {
                return Convert.ToUInt64(numeros).ToString(@"00\.000\.000\/0000\-00");
            }

            return documento ?? string.Empty;
        }
    }
}

using System.Globalization;
using System.Text;

namespace Omnimarket.Api.Utils
{
    public static class SlugHelper
    {
        public static string Gerar(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                return string.Empty;

            var normalized = valor.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();
            var previousWasSeparator = false;

            foreach (var caractere in normalized)
            {
                var categoria = CharUnicodeInfo.GetUnicodeCategory(caractere);

                if (categoria == UnicodeCategory.NonSpacingMark)
                    continue;

                if (char.IsLetterOrDigit(caractere))
                {
                    builder.Append(char.ToLowerInvariant(caractere));
                    previousWasSeparator = false;
                    continue;
                }

                if (previousWasSeparator)
                    continue;

                builder.Append('-');
                previousWasSeparator = true;
            }

            return builder.ToString().Trim('-');
        }
    }
}

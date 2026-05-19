using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;
using System.Text;

namespace Omnimarket.Api.Utils
{
    public static class StringExtensions
    {
        // Remove acentos: "JOÃO" -> "JOAO"
        public static string RemoverAcentos(this string texto)
        {
            if (string.IsNullOrEmpty(texto)) return texto;

            var normalizedString = texto.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        // Normaliza para comparação: " João da Silva " -> "JOAO DA SILVA"
        public static string NormalizarParaComparacao(this string texto)
        {
            if (string.IsNullOrEmpty(texto)) return string.Empty;
            
            return texto.Trim()
                        .ToUpper()
                        .RemoverAcentos();
        }

        // Verifica se os nomes são compatíveis
        public static bool NomeCompativel(string nomeInput, string nomeReceita)
        {
            string n1 = nomeInput.NormalizarParaComparacao();
            string n2 = nomeReceita.NormalizarParaComparacao();

            // 1. Verifica se são idênticos
            if (n1 == n2) return true;

            // 2. Verifica se o primeiro nome e o último sobrenome batem
            // Ex: Input "João Silva", Receita "João Pedro da Silva" -> Passa
            var partes1 = n1.Split(' ');
            var partes2 = n2.Split(' ');

            if (partes1.Length > 1 && partes2.Length > 1)
            {
                bool primeiroNomeBate = partes1[0] == partes2[0];
                bool ultimoNomeBate = partes1.Last() == partes2.Last();

                if (primeiroNomeBate && ultimoNomeBate) return true;
            }

            // 3. Verifica se a Receita contém o nome digitado (parcial)
            if (n2.Contains(n1)) return true;

            return false;
        }
    }
}
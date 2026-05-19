namespace Omnimarket.Api.Utils
{
    public static class CnpjValidador
    {
        public static string Limpar(string? cnpj)
        {
            return new string((cnpj ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());
        }

        public static bool ValidarCnpj(string? cnpj)
        {
            var numeros = Limpar(cnpj);

            if (numeros.Length != 14)
                return false;

            if (numeros.Distinct().Count() == 1)
                return false;

            int[] multiplicador1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var temp = numeros[..12];
            var soma = 0;

            for (var i = 0; i < 12; i++)
                soma += int.Parse(temp[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            var digito = resto < 2 ? 0 : 11 - resto;

            temp += digito;
            soma = 0;

            for (var i = 0; i < 13; i++)
                soma += int.Parse(temp[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            digito = resto < 2 ? 0 : 11 - resto;

            return numeros.EndsWith(digito.ToString());
        }
    }
}

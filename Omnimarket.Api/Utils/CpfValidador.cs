namespace Omnimarket.Api.Utils
{
    public static class CpfValidador
    {
        public static string Limpar(string? cpf)
        {
            return new string((cpf ?? string.Empty)
                .Where(char.IsDigit)
                .ToArray());
        }

        public static bool ValidarCpf(string? cpf)
        {
            var numeros = Limpar(cpf);

            if (string.IsNullOrEmpty(numeros) || numeros.Length != 11)
                return false;

            if (numeros.All(c => c == numeros[0]))
                return false;

            return ValidarDigitosVerificadores(numeros);
        }

        public static string FormatarCpf(string? cpf)
        {
            var numeros = Limpar(cpf);

            if (numeros.Length != 11)
                return string.Empty;

            return $"{numeros[..3]}.{numeros.Substring(3, 3)}.{numeros.Substring(6, 3)}-{numeros.Substring(9, 2)}";
        }

        private static bool ValidarDigitosVerificadores(string cpf)
        {
            int[] multiplicador1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicador2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

            var tempCpf = cpf[..9];

            var soma = 0;
            for (var i = 0; i < 9; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

            var resto = soma % 11;
            var primeiroDigito = resto < 2 ? 0 : 11 - resto;

            tempCpf += primeiroDigito.ToString();

            soma = 0;
            for (var i = 0; i < 10; i++)
                soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

            resto = soma % 11;
            var segundoDigito = resto < 2 ? 0 : 11 - resto;

            return cpf.EndsWith($"{primeiroDigito}{segundoDigito}");
        }
    }
}

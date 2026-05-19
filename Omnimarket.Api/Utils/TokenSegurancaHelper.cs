using System.Security.Cryptography;
using System.Text;

namespace Omnimarket.Api.Utils
{
    public static class TokenSegurancaHelper
    {
        public static string GerarTokenAleatorio(int tamanhoBytes = 48)
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(tamanhoBytes));
        }

        public static string GerarHashSha256(string valor)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(valor ?? string.Empty));
            return Convert.ToHexString(bytes);
        }
    }
}

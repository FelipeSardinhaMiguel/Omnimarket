namespace Omnimarket.Api.Utils
{
    public static class RolesSistema
    {
        public const string User = "User";
        public const string Vendedor = "Vendedor";
        public const string Admin = "Admin";
        public const string Suporte = "Suporte";

        private static readonly string[] RolesValidas =
        {
            User,
            Vendedor,
            Admin,
            Suporte
        };

        public static bool EhValida(string? role)
            => !string.IsNullOrWhiteSpace(Normalizar(role));

        public static string? Normalizar(string? role)
        {
            if (string.IsNullOrWhiteSpace(role))
                return null;

            return RolesValidas.FirstOrDefault(r =>
                string.Equals(r, role.Trim(), StringComparison.OrdinalIgnoreCase));
        }
    }
}

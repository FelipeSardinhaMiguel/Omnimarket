using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public static class AdminSeedService
    {
        public static async Task AplicarAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();

            if (!configuration.GetValue("AdminSeed:Enabled", false))
                return;

            var email = configuration["AdminSeed:Email"]?.Trim().ToLowerInvariant();
            var password = configuration["AdminSeed:Password"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException("Configure AdminSeed:Email e AdminSeed:Password.");

            var context = scope.ServiceProvider.GetRequiredService<DataContext>();
            var usuario = await context.TBL_USUARIO.FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
            {
                var cpf = await ObterCpfDisponivelAsync(context, configuration["AdminSeed:Cpf"]);

                Criptografia.CriarPasswordHash(password, out var hash, out var salt);

                usuario = new Usuario
                {
                    Cpf = cpf,
                    Nome = configuration["AdminSeed:Nome"]?.Trim() ?? "Admin",
                    Sobrenome = configuration["AdminSeed:Sobrenome"]?.Trim() ?? "OmniMarket",
                    Email = email,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    DataCadastro = DateTime.UtcNow,
                    AceitouTermos = true,
                    DataAceiteTermos = DateTime.UtcNow,
                    Role = RolesSistema.Admin
                };

                await context.TBL_USUARIO.AddAsync(usuario);
            }
            else
            {
                usuario.Role = RolesSistema.Admin;

                if (configuration.GetValue("AdminSeed:AtualizarSenha", true))
                {
                    Criptografia.CriarPasswordHash(password, out var hash, out var salt);
                    usuario.PasswordHash = hash;
                    usuario.PasswordSalt = salt;
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task<string> ObterCpfDisponivelAsync(DataContext context, string? cpfConfigurado)
        {
            var cpf = (cpfConfigurado ?? "00000000000")
                .Replace(".", "")
                .Replace("-", "")
                .Trim();

            if (cpf.Length != 11)
                throw new InvalidOperationException("AdminSeed:Cpf deve conter 11 digitos.");

            if (!await context.TBL_USUARIO.AnyAsync(u => u.Cpf == cpf))
                return cpf;

            for (var tentativa = 1; tentativa <= 99; tentativa++)
            {
                var candidato = tentativa.ToString().PadLeft(11, '0');
                if (!await context.TBL_USUARIO.AnyAsync(u => u.Cpf == candidato))
                    return candidato;
            }

            throw new InvalidOperationException("Nao foi possivel encontrar um CPF tecnico livre para o admin seed.");
        }
    }
}

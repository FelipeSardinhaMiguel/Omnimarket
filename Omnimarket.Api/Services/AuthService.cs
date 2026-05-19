using Microsoft.EntityFrameworkCore;
using Omnimarket.Api.Data;
using Omnimarket.Api.Models;
using Omnimarket.Api.Models.Dtos.Usuarios;
using Omnimarket.Api.Models.Dtos.Usuarios.Autenticacao;
using Omnimarket.Api.Utils;

namespace Omnimarket.Api.Services
{
    public class AuthService
    {
        private readonly DataContext _context;
        private readonly TokenService _tokenService;

        public AuthService(DataContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<Usuario> RegistrarUsuario(UsuarioRegistroComContatoDto userDto)
        {
            if (!CpfValidador.ValidarCpf(userDto.Cpf))
                throw new Exception("CPF invalido.");

            var cpfLimpo = userDto.Cpf.Replace(".", "").Replace("-", "").Trim();
            var emailNormalizado = NormalizarEmail(userDto.Email);

            var existe = await _context.TBL_USUARIO
                .AnyAsync(u => u.Cpf == cpfLimpo || u.Email == emailNormalizado);

            if (existe)
                throw new Exception("CPF ou Email ja cadastrado.");

            Criptografia.CriarPasswordHash(userDto.Password, out byte[] hash, out byte[] salt);

            var novoUsuario = new Usuario
            {
                Cpf = cpfLimpo,
                Nome = userDto.Nome.Trim(),
                Sobrenome = userDto.Sobrenome.Trim(),
                Email = emailNormalizado,
                PasswordHash = hash,
                PasswordSalt = salt,
                DataCadastro = DateTime.UtcNow,
                AceitouTermos = userDto.AceitouTermos,
                DataAceiteTermos = userDto.AceitouTermos ? DateTime.UtcNow : null,
                Role = "User"
            };

            if (userDto.Telefones == null || userDto.Telefones.Count == 0)
                throw new Exception("Pelo menos um telefone e obrigatorio.");

            var indiceTelefonePrincipal = userDto.Telefones.FindIndex(t => t.IsPrincipal == true);
            if (indiceTelefonePrincipal < 0)
                indiceTelefonePrincipal = 0;

            for (int i = 0; i < userDto.Telefones.Count; i++)
            {
                var telefoneDto = userDto.Telefones[i];
                var resultado = ValidadorTelefone.ValidarCelularBr(telefoneDto.Ddd, telefoneDto.Numero);

                if (!resultado.Valido)
                    throw new Exception($"Telefone invalido (item {i + 1})");

                novoUsuario.Telefones.Add(new Telefone
                {
                    NumeroE164 = resultado.E164!,
                    IsPrincipal = i == indiceTelefonePrincipal
                });
            }

            if (userDto.Enderecos != null && userDto.Enderecos.Count > 0)
            {
                var indiceEnderecoPrincipal = userDto.Enderecos.FindIndex(e => e.IsPrincipal == true);
                if (indiceEnderecoPrincipal < 0)
                    indiceEnderecoPrincipal = 0;

                for (int i = 0; i < userDto.Enderecos.Count; i++)
                {
                    var enderecoDto = userDto.Enderecos[i];

                    novoUsuario.Enderecos.Add(new Endereco
                    {
                        TipoLogradouro = enderecoDto.TipoLogradouro,
                        NomeEndereco = enderecoDto.NomeEndereco.Trim(),
                        Numero = enderecoDto.Numero.Trim(),
                        Complemento = enderecoDto.Complemento?.Trim(),
                        Cep = enderecoDto.Cep.Replace("-", "").Trim(),
                        Cidade = enderecoDto.Cidade.Trim(),
                        Uf = enderecoDto.Uf.Trim().ToUpperInvariant(),
                        IsPrincipal = i == indiceEnderecoPrincipal,
                        Ativo = true
                    });
                }
            }

            await _context.TBL_USUARIO.AddAsync(novoUsuario);
            await _context.SaveChangesAsync();

            return novoUsuario;
        }

        public async Task<LoginRespostaDto?> Login(LoginDto login)
        {
            var email = NormalizarEmail(login.Email);

            var usuario = await _context.TBL_USUARIO
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null ||
                !Criptografia.VerificarPasswordHash(login.Password, usuario.PasswordHash, usuario.PasswordSalt))
            {
                return null;
            }

            var (token, tokenExpiraEm) = _tokenService.GerarToken(usuario);

            usuario.DataAcesso = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new LoginRespostaDto
            {
                Token = token,
                Nome = $"{usuario.Nome} {usuario.Sobrenome}".Trim(),
                Email = usuario.Email,
                Role = usuario.Role,
                TokenExpiraEm = tokenExpiraEm
            };
        }

        private static string NormalizarEmail(string email)
        {
            return (email ?? string.Empty).Trim().ToLowerInvariant();
        }
    }
}

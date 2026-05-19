using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class AuthServiceTests
{
    [Fact]
    public async Task RegistrarUsuario_DeveSalvarTelefoneEEnderecoAtivo()
    {
        using var fixture = new ServiceTestFixture();
        var dto = fixture.CriarRegistroUsuarioDto("ana");
        dto.Cpf = "529.982.247-25";

        var usuario = await fixture.AuthService.RegistrarUsuario(dto);

        fixture.Context.ChangeTracker.Clear();

        var usuarioSalvo = await fixture.Context.TBL_USUARIO
            .Include(u => u.Telefones)
            .Include(u => u.Enderecos)
            .SingleAsync(u => u.Id == usuario.Id);

        Assert.Equal("52998224725", usuarioSalvo.Cpf);
        Assert.Equal(dto.Email.ToLower().Trim(), usuarioSalvo.Email);
        Assert.Equal("User", usuarioSalvo.Role);
        Assert.True(usuarioSalvo.AceitouTermos);
        Assert.NotNull(usuarioSalvo.DataAceiteTermos);
        Assert.Single(usuarioSalvo.Telefones);
        Assert.True(usuarioSalvo.Telefones[0].IsPrincipal);
        Assert.Single(usuarioSalvo.Enderecos);
        Assert.True(usuarioSalvo.Enderecos[0].Ativo);
        Assert.True(usuarioSalvo.Enderecos[0].IsPrincipal);
    }

    [Fact]
    public async Task RegistrarUsuario_DevePermitirCadastroSemEndereco()
    {
        using var fixture = new ServiceTestFixture();
        var dto = fixture.CriarRegistroUsuarioDto("bruna");
        dto.Enderecos.Clear();

        var usuario = await fixture.AuthService.RegistrarUsuario(dto);

        fixture.Context.ChangeTracker.Clear();

        var usuarioSalvo = await fixture.Context.TBL_USUARIO
            .Include(u => u.Enderecos)
            .SingleAsync(u => u.Id == usuario.Id);

        Assert.Empty(usuarioSalvo.Enderecos);
    }

    [Fact]
    public async Task Login_DeveRetornarTokenComClaimsBasicasDoUsuario()
    {
        using var fixture = new ServiceTestFixture();
        var dto = fixture.CriarRegistroUsuarioDto("bruno");
        await fixture.AuthService.RegistrarUsuario(dto);

        var resposta = await fixture.AuthService.Login(new LoginDto
        {
            Email = dto.Email.ToUpperInvariant(),
            Password = ServiceTestFixture.SenhaPadrao
        });

        Assert.NotNull(resposta);
        Assert.False(string.IsNullOrWhiteSpace(resposta!.Token));
        Assert.Equal(dto.Email.ToLower().Trim(), resposta.Email);
        Assert.Equal("User", resposta.Role);
        Assert.True(resposta.TokenExpiraEm > DateTime.UtcNow);

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(resposta.Token);

        Assert.Contains(jwt.Claims, claim =>
            claim.Type == ClaimTypes.Email &&
            claim.Value == dto.Email.ToLower().Trim());

        Assert.Contains(jwt.Claims, claim =>
            claim.Type == ClaimTypes.Role &&
            claim.Value == "User");

        Assert.DoesNotContain(jwt.Claims, claim => claim.Type == "email_confirmed");
    }

    [Fact]
    public async Task Login_DeveRetornarNuloQuandoSenhaForInvalida()
    {
        using var fixture = new ServiceTestFixture();
        var dto = fixture.CriarRegistroUsuarioDto("clara");
        await fixture.AuthService.RegistrarUsuario(dto);

        var resposta = await fixture.AuthService.Login(new LoginDto
        {
            Email = dto.Email,
            Password = "SenhaErrada@123"
        });

        Assert.Null(resposta);
    }
}

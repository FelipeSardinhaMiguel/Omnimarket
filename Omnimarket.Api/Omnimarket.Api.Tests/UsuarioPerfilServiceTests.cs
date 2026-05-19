using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class UsuarioPerfilServiceTests
{
    [Fact]
    public async Task AtualizarAsync_DeveImpedirEmailDuplicado()
    {
        using var fixture = new ServiceTestFixture();
        var usuario1 = await fixture.CriarUsuarioAsync("usuario-perfil-1");
        var usuario2 = await fixture.CriarUsuarioAsync("usuario-perfil-2");

        var excecao = await Assert.ThrowsAsync<InvalidOperationException>(() => fixture.UsuarioPerfilService.AtualizarAsync(
            usuario1.Id,
            new UsuarioAtualizarDto
            {
                Nome = usuario1.Nome,
                Sobrenome = usuario1.Sobrenome,
                Email = usuario2.Email
            }));

        Assert.Equal("Email ja esta em uso.", excecao.Message);
    }
}

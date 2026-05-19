using Omnimarket.Api.Tests.Support;

namespace Omnimarket.Api.Tests;

public class EnderecoServiceTests
{
    [Fact]
    public async Task RemoverAsync_DevePromoverOutroEnderecoQuandoPrincipalForDesativado()
    {
        using var fixture = new ServiceTestFixture();
        var usuario = await fixture.CriarUsuarioAsync("usuario-endereco");
        var enderecoPrincipal = await fixture.CriarEnderecoAsync(usuario.Id, principal: true);
        var enderecoSecundario = await fixture.CriarEnderecoAsync(usuario.Id, principal: false);

        var removido = await fixture.EnderecoService.RemoverAsync(usuario.Id, enderecoPrincipal.Id);

        fixture.Context.ChangeTracker.Clear();

        var principalAtualizado = await fixture.Context.TBL_ENDERECO
            .SingleAsync(e => e.Id == enderecoPrincipal.Id);

        var secundarioAtualizado = await fixture.Context.TBL_ENDERECO
            .SingleAsync(e => e.Id == enderecoSecundario.Id);

        Assert.True(removido);
        Assert.False(principalAtualizado.Ativo);
        Assert.False(principalAtualizado.IsPrincipal);
        Assert.True(secundarioAtualizado.IsPrincipal);
    }
}

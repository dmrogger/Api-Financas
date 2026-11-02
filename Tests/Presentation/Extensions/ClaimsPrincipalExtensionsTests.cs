using ApiFinancas.Src.Presentation.Extensions;
using System.Security.Claims;

namespace ApiFinancas.Tests.Presentation.Extensions
{
    public class ClaimsPrincipalExtensionsTests
    {
        private static ClaimsPrincipal CriarUsuario(params Claim[] claims)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }

        [Fact(DisplayName = "Deve obter o id do usuário quando a claim for válida")]
        public void DeveObterIdUsuarioQuandoClaimValida()
        {
            var idEsperado = Guid.NewGuid();
            var usuario = CriarUsuario(new Claim(ClaimTypes.NameIdentifier, idEsperado.ToString()));

            var sucesso = usuario.TryObterIdUsuario(out var idUsuario);

            Assert.True(sucesso);
            Assert.Equal(idEsperado, idUsuario);
        }

        [Fact(DisplayName = "Não deve obter o id do usuário quando a claim não existir")]
        public void NaoDeveObterIdUsuarioQuandoClaimNaoExistir()
        {
            var usuario = CriarUsuario(new Claim(ClaimTypes.Email, "teste@gmail.com"));

            var sucesso = usuario.TryObterIdUsuario(out var idUsuario);

            Assert.False(sucesso);
            Assert.Equal(Guid.Empty, idUsuario);
        }

        [Theory(DisplayName = "Não deve obter o id do usuário quando a claim for inválida")]
        [InlineData("")]
        [InlineData("texto-invalido")]
        [InlineData("12345")]
        public void NaoDeveObterIdUsuarioQuandoClaimInvalida(string valorClaim)
        {
            var usuario = CriarUsuario(new Claim(ClaimTypes.NameIdentifier, valorClaim));

            var sucesso = usuario.TryObterIdUsuario(out var idUsuario);

            Assert.False(sucesso);
            Assert.Equal(Guid.Empty, idUsuario);
        }
    }
}

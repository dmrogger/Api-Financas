using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiFinancas.Tests.Infrastructure.Security
{
    public class JwtTokenServiceTests
    {
        private const string Chave = "uma-chave-super-secreta-para-testes";
        private const string Issuer = "ApiFinancas";
        private const string Audience = "ApiFinancasUsers";

        private readonly Usuario _usuario = new Usuario("Teste", "teste@gmail.com", "hash");

        private static JwtTokenService CriarServico(string? chave = Chave, string? issuer = Issuer, string? audience = Audience)
        {
            var configuracoes = new Dictionary<string, string?>
            {
                { "Jwt:Key", chave },
                { "Jwt:Issuer", issuer },
                { "Jwt:Audience", audience }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(configuracoes)
                .Build();

            return new JwtTokenService(configuration);
        }

        private static TokenValidationParameters CriarParametrosDeValidacao(string chave)
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = Issuer,
                ValidateAudience = true,
                ValidAudience = Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
                ValidateLifetime = true
            };
        }

        [Fact(DisplayName = "Deve gerar token válido com os dados do usuário")]
        public void DeveGerarTokenValidoComDadosDoUsuario()
        {
            var token = CriarServico().GerarToken(_usuario);

            var principal = new JwtSecurityTokenHandler()
                .ValidateToken(token, CriarParametrosDeValidacao(Chave), out _);

            Assert.Equal(_usuario.Id.ToString(), principal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            Assert.Equal(_usuario.Email, principal.FindFirst(ClaimTypes.Email)?.Value);
            Assert.Equal(_usuario.Nome, principal.FindFirst(ClaimTypes.Name)?.Value);
        }

        [Fact(DisplayName = "Deve gerar token com validade de duas horas")]
        public void DeveGerarTokenComValidadeDeDuasHoras()
        {
            var antes = DateTime.UtcNow;

            var token = CriarServico().GerarToken(_usuario);

            var depois = DateTime.UtcNow;
            var tokenLido = new JwtSecurityTokenHandler().ReadJwtToken(token);

            // O JWT guarda a expiração em segundos inteiros, por isso a folga de 1 segundo
            Assert.InRange(tokenLido.ValidTo, antes.AddHours(2).AddSeconds(-1), depois.AddHours(2));
        }

        [Fact(DisplayName = "Não deve validar o token com outra chave")]
        public void NaoDeveValidarTokenComOutraChave()
        {
            var token = CriarServico().GerarToken(_usuario);
            var parametros = CriarParametrosDeValidacao("outra-chave-super-secreta-para-testes");

            Assert.ThrowsAny<SecurityTokenException>(() =>
                new JwtSecurityTokenHandler().ValidateToken(token, parametros, out _));
        }

        [Fact(DisplayName = "Não deve gerar token para usuário nulo")]
        public void NaoDeveGerarTokenParaUsuarioNulo()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => CriarServico().GerarToken(null!));

            Assert.Equal("usuario", excecao.ParamName);
        }

        [Fact(DisplayName = "Não deve gerar token sem chave configurada")]
        public void NaoDeveGerarTokenSemChaveConfigurada()
        {
            var excecao = Assert.Throws<InvalidOperationException>(() =>
                CriarServico(chave: null).GerarToken(_usuario));

            Assert.Equal("A chave JWT não foi configurada.", excecao.Message);
        }

        [Fact(DisplayName = "Não deve gerar token sem issuer configurado")]
        public void NaoDeveGerarTokenSemIssuerConfigurado()
        {
            var excecao = Assert.Throws<InvalidOperationException>(() =>
                CriarServico(issuer: null).GerarToken(_usuario));

            Assert.Equal("O Issuer JWT não foi configurado.", excecao.Message);
        }

        [Fact(DisplayName = "Não deve gerar token sem audience configurado")]
        public void NaoDeveGerarTokenSemAudienceConfigurado()
        {
            var excecao = Assert.Throws<InvalidOperationException>(() =>
                CriarServico(audience: null).GerarToken(_usuario));

            Assert.Equal("O Audience JWT não foi configurado.", excecao.Message);
        }

        [Theory(DisplayName = "Não deve gerar token com chave menor que 32 bytes")]
        [InlineData("")]
        [InlineData("chave-curta")]
        [InlineData("1234567890123456789012345678901")]
        public void NaoDeveGerarTokenComChaveCurta(string chave)
        {
            var excecao = Assert.Throws<InvalidOperationException>(() =>
                CriarServico(chave: chave).GerarToken(_usuario));

            Assert.Equal("A chave JWT deve possuir pelo menos 32 bytes.", excecao.Message);
        }
    }
}

using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.Interfaces.Segurança;
using ApiFinancas.Src.Application.Services.Autenticacao;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using Moq;

namespace ApiFinancas.Tests.Application.Services.Autenticacao
{
    public class AutenticacaoServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly Mock<ISenhaService> _senhaServiceMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AutenticacaoService _autenticaoservice;

        public AutenticacaoServiceTests()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _senhaServiceMock = new Mock<ISenhaService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _autenticaoservice = new AutenticacaoService(
                _repositoryMock.Object,
                _senhaServiceMock.Object,
                _tokenServiceMock.Object);
        }

        [Fact(DisplayName = "Deve retornar login com usuário válido")]
        public async Task LoginDeveRetornarUsuarioValido()
        {
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            _repositoryMock.Setup(x => x.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha("123456", usuario.Senha))
                .Returns(true);

            _tokenServiceMock.Setup(x => x.GerarToken(usuario))
                .Returns("token-teste");

            var login = new LoginRequest
            {
                Email = "teste@gmail.com",
                Senha = "123456"
            };

            var result = await _autenticaoservice.LoginAsync(login);


            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(result.Data.Email, login.Email);
            Assert.False(string.IsNullOrEmpty(result.Data.Token));
        }

        [Fact(DisplayName = "Deve retornar um login inválido e não gerar token")]
        public async Task LoginDeveRetornarInvalido()
        {
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            _repositoryMock.Setup(x => x.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha(It.IsAny<string>(), usuario.Senha))
                .Returns(false); //Senha Incorreta para verificação

            var login = new LoginRequest
            {
                Email = "teste@gmail.com",
                Senha = "123456"
            };

            var result = await _autenticaoservice.LoginAsync(login);

            var esperado = "Usuário ou senha inválidos";

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal(result.Error, esperado);
            Assert.Null(result.Data);
            _tokenServiceMock.Verify(x => x.GerarToken(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar um login inválido quando não existir")]
        public async Task LoginAsync_DeveRetornarErro_QuandoUsuarioNaoExistir()
        {
            _repositoryMock.Setup(x => x.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((Usuario)null!);

            var login = new LoginRequest
            {
                Email = "teste@gmail.com",
                Senha = "123456"
            };

            var result = await _autenticaoservice.LoginAsync(login);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Usuário ou senha inválidos", result.Error);
            _tokenServiceMock.Verify(x => x.GerarToken(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact(DisplayName = "Deve gerar token no login válido")]
        public async Task LoginDeveGerarToken()
        {
            var usuario = new Usuario("Teste", "teste@gmail.com", "hash");

            _repositoryMock.Setup(x => x.ObterPorEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha("123456", usuario.Senha))
                .Returns(true);

            _tokenServiceMock.Setup(x => x.GerarToken(usuario))
                .Returns("token-teste");

            var login = new LoginRequest
            {
                Email = "teste@gmail.com",
                Senha = "123456"
            };

            var result = await _autenticaoservice.LoginAsync(login);

            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("token-teste", result.Data.Token);
            _tokenServiceMock.Verify(x => x.GerarToken(usuario), Times.Once);
        }
    }
}

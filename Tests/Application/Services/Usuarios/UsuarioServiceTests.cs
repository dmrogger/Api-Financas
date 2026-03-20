using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.Interfaces.Segurança;
using ApiFinancas.Src.Application.Services.Usuarios;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using ApiFinancas.Src.Infrastructure.Security;
using Moq;

namespace ApiFinancas.Tests.Application.Services.Usuarios
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _repositoryMock;
        private readonly Mock<ISenhaService> _senhaServiceMock;
        private readonly UsuarioService _usuarioService;


        public UsuarioServiceTests()
        {
            _repositoryMock = new Mock<IUsuarioRepository>();
            _senhaServiceMock = new Mock<ISenhaService>();

            _usuarioService = new UsuarioService(_repositoryMock.Object, _senhaServiceMock.Object);
        }

        [Fact(DisplayName = "Deve criar um usuário válido")]
        public async Task DeveCriarUmUsuarioComSucesso()
        {
            var request = new CriaUsuarioRequest(
                "teste@email.com",
                "Teste da Silva", 
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync((Usuario?)null);

            _senhaServiceMock
                .Setup(x => x.HashSenha(request.Senha))
                .Returns("hash");

            _repositoryMock
                .Setup(x => x.AdicionarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync(Guid.NewGuid());

            var result = await _usuarioService.CriarUsuarioAsync(request);


            Assert.True(result.Success);
            Assert.Equal(request.Email, result.Data!.Email);
            Assert.Equal(request.Nome, result.Data.Nome);
            Assert.NotEqual(result.Data.idUsuario, Guid.Empty);
            Assert.Null(result.Error);
        }

    }
}

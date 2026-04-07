using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Common;
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


        [Fact(DisplayName = "Deve retornar mensagem de usuário já cadastrado")]
        public async Task DeveRetornarUsuarioJaCadastrado()
        {
            var request = new CriaUsuarioRequest(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            var Usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            var mensagemEsperada = "E-mail já cadastrado! Tente criar a conta usando outro E-mail";

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync((Usuario));

            _senhaServiceMock
                .Setup(x => x.HashSenha(request.Senha))
                .Returns("hash");

            _repositoryMock
                .Setup(x => x.AdicionarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync(Guid.NewGuid());

            var result = await _usuarioService.CriarUsuarioAsync(request);

            Assert.False(result.Success);
            Assert.Equal(result.Error, mensagemEsperada);
            Assert.Null(result.Data);
        }


        [Fact(DisplayName = "Deve retornar mensagem de erro desconhecido")]
        public async Task DeveRetornarErroDesconhecido()
        {
            var request = new CriaUsuarioRequest(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            var mensagemEsperada = "Erro desconhecido ao criar usuário!";

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync((Usuario?)null);

            _senhaServiceMock
                .Setup(x => x.HashSenha(request.Senha))
                .Returns("hash");

            _repositoryMock
                .Setup(x => x.AdicionarAsync(It.IsAny<Usuario>()))
                .ReturnsAsync(Guid.Empty);

            var result = await _usuarioService.CriarUsuarioAsync(request);

            Assert.False(result.Success);
            Assert.Equal(result.Error, mensagemEsperada);
            Assert.Null(result.Data);
        }

        [Fact(DisplayName = "Deve atualizar a senha do usuário com sucesso")]
        public async Task DeveAtualizarSenhaComSucesso()
        {
            var request = new EditaUsuarioRequest { 
                Email = "teste@email.com", 
                SenhaAtual = "teste123", 
                SenhaNova = "teste321" };

            var Usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(Usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha(request.SenhaAtual, Usuario.Senha))
                .Returns(true);

            _repositoryMock.Setup(x => x.AtualizarSenhaAsync(Usuario))
                .Returns(Task.CompletedTask);

            var result = await _usuarioService.AtualizaSenha(request);

            Assert.True(result.Success);
            Assert.Null(result.Error);
        }


    }
}

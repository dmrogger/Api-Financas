using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.Interfaces.Segurança;
using ApiFinancas.Src.Application.Services.Usuarios;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
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
            Assert.Equal("Senha alterada com sucesso!", result.Data);
        }

        [Fact(DisplayName = "Não deve atualizar a senha e deve retornar erro quando usuário não localizado")]
        public async Task NaoDeveAtualizarSenhaeRetornarErroQuandoUsuarioNaoLocalizado()
        {
            var request = new EditaUsuarioRequest
            {
                Email = "teste@email.com",
                SenhaAtual = "teste123",
                SenhaNova = "teste321"
            };

            var Usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync((Usuario?)null);

            _senhaServiceMock.Setup(x => x.ValidaSenha(request.SenhaAtual, Usuario.Senha))
                .Returns(true);

            _repositoryMock.Setup(x => x.AtualizarSenhaAsync(Usuario))
                .Returns(Task.CompletedTask);

            var result = await _usuarioService.AtualizaSenha(request);

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("Usuário não localizado!", result.Error);
        }


        [Fact(DisplayName = "Não deve atualizar a senha e deve retornar erro quando email ou senha inválidos")]
        public async Task NaoDeveAtualizarSenhaeRetornarErroQuando()
        {
            var request = new EditaUsuarioRequest
            {
                Email = "teste@email.com",
                SenhaAtual = "teste123",
                SenhaNova = "teste321"
            };

            var usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha(request.SenhaAtual, usuario.Senha))
                .Returns(false);

            _repositoryMock.Setup(x => x.AtualizarSenhaAsync(usuario))
                .Returns(Task.CompletedTask);

            var result = await _usuarioService.AtualizaSenha(request);

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("Erro: Email ou senha inválidos!", result.Error);
        }


        [Fact(DisplayName = "Não deve atualizar a senha quando houver erro interno")]
        public async Task NaoDeveAtualizarSenhaQuandoErroInterno()
        {
            var request = new EditaUsuarioRequest
            {
                Email = "teste@email.com",
                SenhaAtual = "teste123",
                SenhaNova = "teste321"
            };

            var usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha(request.SenhaAtual, usuario.Senha))
                .Returns(true);

            _repositoryMock.Setup(x => x.AtualizarSenhaAsync(usuario))
                .ThrowsAsync(new Exception("Erro interno qualquer"));

            var result = await _usuarioService.AtualizaSenha(request);

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("Erro interno ao alterar senha do usuário", result.Error);
        }


        [Fact(DisplayName = "Deve deletar usuario com sucesso")]
        public async Task DeveDeletarUmusuarioComSucesso()
        {
            var request = new ExcluiUsuarioRequest
            {
                Email = "teste@email.com",
                Senha = "teste123"
            };

            var usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha(request.Senha, usuario.Senha))
                 .Returns(true);

            _repositoryMock
                .Setup(x => x.DeletarAsync(usuario));

            var result = await _usuarioService.DeletaUsuario(request);

            Assert.True(result.Success);
            Assert.Null(result.Error);
            Assert.Equal("Usuário excluído com sucesso!", result.Data);
        }


        [Fact(DisplayName = "Não deve deletar o usuário e deve retornar erro de senha")]
        public async Task NaoDeveDeletarUsuarioeDeveRetornarErroDeSenha()
        {
            var request = new ExcluiUsuarioRequest
            {
                Email = "teste@email.com",
                Senha = "teste123"
            };

            var usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha(request.Senha, usuario.Senha))
                 .Returns(false);

            _repositoryMock
                .Setup(x => x.DeletarAsync(usuario));

            var result = await _usuarioService.DeletaUsuario(request);

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("Erro: Email ou senha inválidos!", result.Error);
        }


        [Fact(DisplayName = "Não deve deletar o usuário e deve retornar erro de usuario não localizado")]
        public async Task NaoDeveDeletarUsuarioeDeveRetornarErroUsuarioNaoLocalizado()
        {
            var request = new ExcluiUsuarioRequest
            {
                Email = "teste@email.com",
                Senha = "teste123"
            };

            var usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync((Usuario?)null);

            _senhaServiceMock.Setup(x => x.ValidaSenha(request.Senha, usuario.Senha))
                 .Returns(true);

            _repositoryMock
                .Setup(x => x.DeletarAsync(usuario));

            var result = await _usuarioService.DeletaUsuario(request);

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("Usuário não localizado, não foi possível deletar", result.Error);
        }


        [Fact(DisplayName = "Não deve deletar o usuário e deve retornar erro interno")]
        public async Task NaoDeveDeletarUsuarioeDeveRetornarErroInterno()
        {
            var request = new ExcluiUsuarioRequest
            {
                Email = "teste@email.com",
                Senha = "teste123"
            };

            var usuario = new Usuario(
                "teste@email.com",
                "Teste da Silva",
                "teste123");

            _repositoryMock
                .Setup(x => x.ObterPorEmailAsync(request.Email))
                .ReturnsAsync(usuario);

            _senhaServiceMock.Setup(x => x.ValidaSenha(request.Senha, usuario.Senha))
                 .Returns(true);

            _repositoryMock
                .Setup(x => x.DeletarAsync(usuario))
                .ThrowsAsync(new Exception("Erro interno qualquer"));

            var result = await _usuarioService.DeletaUsuario(request);

            Assert.False(result.Success);
            Assert.NotNull(result.Error);
            Assert.Equal("Erro interno ao excluír o usuário", result.Error);
        }
    }
}

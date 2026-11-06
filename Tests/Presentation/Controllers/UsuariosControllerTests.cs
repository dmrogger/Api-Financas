using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;
using ApiFinancas.Src.Application.Interfaces.Usuario;
using ApiFinancas.Src.Presentation.Controllers.Usuario;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ApiFinancas.Tests.Presentation.Controllers
{
    public class UsuariosControllerTests
    {
        private readonly Mock<IUsuarioService> _usuarioServiceMock;
        private readonly UsuariosController _controller;
        private readonly Guid _idUsuario = Guid.NewGuid();
        private readonly CriaUsuarioRequest _criacaoRequest =
            new CriaUsuarioRequest("teste@gmail.com", "Teste", "Senha@123");
        private readonly EditaUsuarioRequest _edicaoRequest = new EditaUsuarioRequest
        {
            SenhaAtual = "Senha@123",
            Nome = "Nome Novo"
        };
        private readonly ExcluiUsuarioRequest _exclusaoRequest = new ExcluiUsuarioRequest
        {
            Senha = "Senha@123"
        };

        public UsuariosControllerTests()
        {
            _usuarioServiceMock = new Mock<IUsuarioService>();
            _controller = new UsuariosController(_usuarioServiceMock.Object);
            DefinirUsuario(new Claim(ClaimTypes.NameIdentifier, _idUsuario.ToString()));
        }

        private void DefinirUsuario(params Claim[] claims)
        {
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(claims))
                }
            };
        }

        [Fact(DisplayName = "Deve retornar 201 ao criar usuário")]
        public async Task DeveRetornarCreatedAoCriarUsuario()
        {
            var resultado = Result<UsuarioResponse>.Ok(new UsuarioResponse(_idUsuario, "Teste", "teste@gmail.com"));

            _usuarioServiceMock.Setup(x => x.CriarUsuarioAsync(_criacaoRequest)).ReturnsAsync(resultado);

            var resposta = await _controller.CriarUsuario(_criacaoRequest);

            var criado = Assert.IsType<CreatedAtActionResult>(resposta);
            Assert.Equal(nameof(UsuariosController.ConsultarUsuario), criado.ActionName);
            Assert.Same(resultado, criado.Value);
        }

        [Fact(DisplayName = "Deve retornar 400 quando criação falhar")]
        public async Task DeveRetornarBadRequestQuandoCriacaoFalhar()
        {
            var resultado = Result<UsuarioResponse>.Fail("Erro desconhecido ao criar usuário!");

            _usuarioServiceMock.Setup(x => x.CriarUsuarioAsync(_criacaoRequest)).ReturnsAsync(resultado);

            var resposta = await _controller.CriarUsuario(_criacaoRequest);

            var requisicaoInvalida = Assert.IsType<BadRequestObjectResult>(resposta);
            Assert.Same(resultado, requisicaoInvalida.Value);
        }

        [Fact(DisplayName = "Deve retornar 409 quando e-mail já estiver cadastrado")]
        public async Task DeveRetornarConflictQuandoEmailJaCadastrado()
        {
            var resultado = Result<UsuarioResponse>.Fail(
                "E-mail já cadastrado! Tente criar a conta usando outro E-mail", ETipoErro.Conflito);

            _usuarioServiceMock.Setup(x => x.CriarUsuarioAsync(_criacaoRequest)).ReturnsAsync(resultado);

            var resposta = await _controller.CriarUsuario(_criacaoRequest);

            var conflito = Assert.IsType<ConflictObjectResult>(resposta);
            Assert.Same(resultado, conflito.Value);
        }

        [Fact(DisplayName = "Deve retornar 200 ao consultar usuário")]
        public async Task DeveRetornarOkAoConsultarUsuario()
        {
            var resultado = Result<UsuarioResponse>.Ok(new UsuarioResponse(_idUsuario, "Teste", "teste@gmail.com"));

            _usuarioServiceMock.Setup(x => x.ConsultaUsuario(_idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.ConsultarUsuario();

            var ok = Assert.IsType<OkObjectResult>(resposta);
            Assert.Same(resultado, ok.Value);
        }

        [Fact(DisplayName = "Deve retornar 404 quando usuário não for localizado")]
        public async Task DeveRetornarNotFoundQuandoUsuarioNaoLocalizado()
        {
            var resultado = Result<UsuarioResponse>.Fail("Usuário não localizado!", ETipoErro.NaoEncontrado);

            _usuarioServiceMock.Setup(x => x.ConsultaUsuario(_idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.ConsultarUsuario();

            var naoEncontrado = Assert.IsType<NotFoundObjectResult>(resposta);
            Assert.Same(resultado, naoEncontrado.Value);
        }

        [Fact(DisplayName = "Deve retornar 401 ao consultar com token inválido")]
        public async Task DeveRetornarUnauthorizedAoConsultarComTokenInvalido()
        {
            DefinirUsuario();

            var resposta = await _controller.ConsultarUsuario();

            var naoAutorizado = Assert.IsType<UnauthorizedObjectResult>(resposta);
            Assert.Equal("Token inválido ou incompleto!", naoAutorizado.Value);
            _usuarioServiceMock.Verify(x => x.ConsultaUsuario(It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar 200 ao editar usuário")]
        public async Task DeveRetornarOkAoEditarUsuario()
        {
            var resultado = Result<string>.Ok("Usuário atualizado com sucesso!");

            _usuarioServiceMock.Setup(x => x.EditaUsuario(_edicaoRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.EditaUsuario(_edicaoRequest);

            var ok = Assert.IsType<OkObjectResult>(resposta);
            Assert.Same(resultado, ok.Value);
        }

        [Fact(DisplayName = "Deve retornar 400 quando edição falhar")]
        public async Task DeveRetornarBadRequestQuandoEdicaoFalhar()
        {
            var resultado = Result<string>.Fail("Erro: Email ou senha inválidos!");

            _usuarioServiceMock.Setup(x => x.EditaUsuario(_edicaoRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.EditaUsuario(_edicaoRequest);

            var requisicaoInvalida = Assert.IsType<BadRequestObjectResult>(resposta);
            Assert.Same(resultado, requisicaoInvalida.Value);
        }

        [Fact(DisplayName = "Deve retornar 404 ao editar usuário não localizado")]
        public async Task DeveRetornarNotFoundAoEditarUsuarioNaoLocalizado()
        {
            var resultado = Result<string>.Fail("Usuário não localizado!", ETipoErro.NaoEncontrado);

            _usuarioServiceMock.Setup(x => x.EditaUsuario(_edicaoRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.EditaUsuario(_edicaoRequest);

            var naoEncontrado = Assert.IsType<NotFoundObjectResult>(resposta);
            Assert.Same(resultado, naoEncontrado.Value);
        }

        [Fact(DisplayName = "Deve retornar 401 ao editar com token inválido")]
        public async Task DeveRetornarUnauthorizedAoEditarComTokenInvalido()
        {
            DefinirUsuario(new Claim(ClaimTypes.NameIdentifier, "id-invalido"));

            var resposta = await _controller.EditaUsuario(_edicaoRequest);

            var naoAutorizado = Assert.IsType<UnauthorizedObjectResult>(resposta);
            Assert.Equal("Token inválido ou incompleto!", naoAutorizado.Value);
            _usuarioServiceMock.Verify(x => x.EditaUsuario(It.IsAny<EditaUsuarioRequest>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar 200 ao excluir usuário")]
        public async Task DeveRetornarOkAoExcluirUsuario()
        {
            var resultado = Result<string>.Ok("Usuário excluído com sucesso!");

            _usuarioServiceMock.Setup(x => x.DeletaUsuario(_exclusaoRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.DeletarUsuario(_exclusaoRequest);

            var ok = Assert.IsType<OkObjectResult>(resposta);
            Assert.Same(resultado, ok.Value);
        }

        [Fact(DisplayName = "Deve retornar 400 quando exclusão falhar")]
        public async Task DeveRetornarBadRequestQuandoExclusaoFalhar()
        {
            var resultado = Result<string>.Fail("Erro: Email ou senha inválidos!");

            _usuarioServiceMock.Setup(x => x.DeletaUsuario(_exclusaoRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.DeletarUsuario(_exclusaoRequest);

            var requisicaoInvalida = Assert.IsType<BadRequestObjectResult>(resposta);
            Assert.Same(resultado, requisicaoInvalida.Value);
        }

        [Fact(DisplayName = "Deve retornar 404 ao excluir usuário não localizado")]
        public async Task DeveRetornarNotFoundAoExcluirUsuarioNaoLocalizado()
        {
            var resultado = Result<string>.Fail(
                "Usuário não localizado, não foi possível deletar", ETipoErro.NaoEncontrado);

            _usuarioServiceMock.Setup(x => x.DeletaUsuario(_exclusaoRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.DeletarUsuario(_exclusaoRequest);

            var naoEncontrado = Assert.IsType<NotFoundObjectResult>(resposta);
            Assert.Same(resultado, naoEncontrado.Value);
        }

        [Fact(DisplayName = "Deve retornar 401 ao excluir com token inválido")]
        public async Task DeveRetornarUnauthorizedAoExcluirComTokenInvalido()
        {
            DefinirUsuario(new Claim(ClaimTypes.NameIdentifier, "id-invalido"));

            var resposta = await _controller.DeletarUsuario(_exclusaoRequest);

            var naoAutorizado = Assert.IsType<UnauthorizedObjectResult>(resposta);
            Assert.Equal("Token inválido ou incompleto!", naoAutorizado.Value);
            _usuarioServiceMock.Verify(x => x.DeletaUsuario(It.IsAny<ExcluiUsuarioRequest>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}

using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Categorias;
using ApiFinancas.Src.Application.DTOs.Responses.Categorias;
using ApiFinancas.Src.Application.Interfaces.Categorias;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Presentation.Controllers.Categorias;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ApiFinancas.Tests.Presentation.Controllers
{
    public class CategoriasControllerTests
    {
        private readonly Mock<ICategoriaService> _serviceMock;
        private readonly CategoriasController _controller;
        private readonly Guid _idUsuario = Guid.NewGuid();
        private readonly CategoriaRequest _request = new CategoriaRequest { Nome = "Pets" };

        public CategoriasControllerTests()
        {
            _serviceMock = new Mock<ICategoriaService>();
            _controller = new CategoriasController(_serviceMock.Object);
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

        private CategoriaResponse Resposta() => new CategoriaResponse(new Categoria("Pets", _idUsuario));

        [Fact(DisplayName = "Não deve instanciar o controller sem o serviço")]
        public void NaoDeveInstanciarSemServico()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new CategoriasController(null!));

            Assert.Equal("categoriaService", excecao.ParamName);
        }

        [Fact(DisplayName = "Deve retornar 200 ao listar categorias")]
        public async Task DeveRetornarOkAoListar()
        {
            var resultado = Result<List<CategoriaResponse>>.Ok(new List<CategoriaResponse> { Resposta() });
            _serviceMock.Setup(x => x.ListarAsync(_idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.Listar();

            var ok = Assert.IsType<OkObjectResult>(resposta);
            Assert.Same(resultado, ok.Value);
        }

        [Fact(DisplayName = "Deve retornar 201 ao criar categoria")]
        public async Task DeveRetornarCreatedAoCriar()
        {
            var resultado = Result<CategoriaResponse>.Ok(Resposta());
            _serviceMock.Setup(x => x.CriarAsync(_request, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.Criar(_request);

            var criado = Assert.IsType<ObjectResult>(resposta);
            Assert.Equal(StatusCodes.Status201Created, criado.StatusCode);
            Assert.Same(resultado, criado.Value);
        }

        [Fact(DisplayName = "Deve retornar 409 ao criar categoria com nome duplicado")]
        public async Task DeveRetornarConflictAoCriarDuplicada()
        {
            var resultado = Result<CategoriaResponse>.Fail("Já existe uma categoria com esse nome.", ETipoErro.Conflito);
            _serviceMock.Setup(x => x.CriarAsync(_request, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.Criar(_request);

            var conflito = Assert.IsType<ConflictObjectResult>(resposta);
            Assert.Same(resultado, conflito.Value);
        }

        [Fact(DisplayName = "Deve retornar 200 ao editar categoria")]
        public async Task DeveRetornarOkAoEditar()
        {
            var id = Guid.NewGuid();
            var resultado = Result<CategoriaResponse>.Ok(Resposta());
            _serviceMock.Setup(x => x.EditarAsync(id, _request, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.Editar(id, _request);

            var ok = Assert.IsType<OkObjectResult>(resposta);
            Assert.Same(resultado, ok.Value);
        }

        [Fact(DisplayName = "Deve retornar 403 ao editar categoria padrão")]
        public async Task DeveRetornarForbiddenAoEditarPadrao()
        {
            var id = Guid.NewGuid();
            var resultado = Result<CategoriaResponse>.Fail("Categorias padrão não podem ser alteradas nem excluídas.", ETipoErro.Proibido);
            _serviceMock.Setup(x => x.EditarAsync(id, _request, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.Editar(id, _request);

            var proibido = Assert.IsType<ObjectResult>(resposta);
            Assert.Equal(StatusCodes.Status403Forbidden, proibido.StatusCode);
            Assert.Same(resultado, proibido.Value);
        }

        [Fact(DisplayName = "Deve retornar 404 ao editar categoria não encontrada")]
        public async Task DeveRetornarNotFoundAoEditarInexistente()
        {
            var id = Guid.NewGuid();
            var resultado = Result<CategoriaResponse>.Fail("Categoria não encontrada.", ETipoErro.NaoEncontrado);
            _serviceMock.Setup(x => x.EditarAsync(id, _request, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.Editar(id, _request);

            Assert.IsType<NotFoundObjectResult>(resposta);
        }

        [Fact(DisplayName = "Deve retornar 204 ao excluir categoria")]
        public async Task DeveRetornarNoContentAoDeletar()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(x => x.DeletarAsync(id, _idUsuario))
                .ReturnsAsync(Result<string>.Ok("Categoria excluída com sucesso!"));

            var resposta = await _controller.Deletar(id);

            Assert.IsType<NoContentResult>(resposta);
        }

        [Fact(DisplayName = "Deve retornar 404 ao excluir categoria não encontrada")]
        public async Task DeveRetornarNotFoundAoDeletarInexistente()
        {
            var id = Guid.NewGuid();
            _serviceMock.Setup(x => x.DeletarAsync(id, _idUsuario))
                .ReturnsAsync(Result<string>.Fail("Categoria não encontrada.", ETipoErro.NaoEncontrado));

            var resposta = await _controller.Deletar(id);

            Assert.IsType<NotFoundObjectResult>(resposta);
        }

        [Fact(DisplayName = "Deve retornar 401 com token inválido")]
        public async Task DeveRetornarUnauthorizedComTokenInvalido()
        {
            DefinirUsuario(new Claim(ClaimTypes.NameIdentifier, "id-invalido"));

            Assert.IsType<UnauthorizedObjectResult>(await _controller.Listar());
            Assert.IsType<UnauthorizedObjectResult>(await _controller.Criar(_request));
            Assert.IsType<UnauthorizedObjectResult>(await _controller.Editar(Guid.NewGuid(), _request));
            Assert.IsType<UnauthorizedObjectResult>(await _controller.Deletar(Guid.NewGuid()));

            _serviceMock.Verify(x => x.ListarAsync(It.IsAny<Guid>()), Times.Never);
            _serviceMock.Verify(x => x.CriarAsync(It.IsAny<CategoriaRequest>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}

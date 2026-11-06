using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes;
using ApiFinancas.Src.Application.Interfaces.Movimentacoes;
using ApiFinancas.Src.Domain.Enums;
using ApiFinancas.Src.Presentation.Controllers.Movimentacoes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ApiFinancas.Tests.Presentation.Controllers
{
    public class MovimentacoesControllerTests
    {
        private readonly Mock<IMovimentacaoService> _movimentacaoServiceMock;
        private readonly MovimentacoesController _controller;
        private readonly Guid _idUsuario = Guid.NewGuid();
        private readonly CadastraMovimentacoesRequest _cadastroRequest =
            new CadastraMovimentacoesRequest(100, "Teste", null, ETipoMovimentacao.Entrada);
        private readonly ObterMovimentacoesRequest _consultaRequest = new ObterMovimentacoesRequest();

        public MovimentacoesControllerTests()
        {
            _movimentacaoServiceMock = new Mock<IMovimentacaoService>();
            _controller = new MovimentacoesController(_movimentacaoServiceMock.Object);
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

        [Fact(DisplayName = "Não deve instanciar o controller sem o serviço")]
        public void NaoDeveInstanciarSemServico()
        {
            var excecao = Assert.Throws<ArgumentNullException>(() => new MovimentacoesController(null!));

            Assert.Equal("movimentacaoService", excecao.ParamName);
        }

        [Fact(DisplayName = "Deve retornar 201 ao cadastrar movimentação")]
        public async Task DeveRetornarCreatedAoCadastrar()
        {
            var resultado = Result<CadastrarMovimentacoesResponse>.Ok(new CadastrarMovimentacoesResponse(Guid.NewGuid()));

            _movimentacaoServiceMock.Setup(x => x.CadastraMovimentacao(_cadastroRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.CriaMovimentacao(_cadastroRequest);

            var criado = Assert.IsType<ObjectResult>(resposta);
            Assert.Equal(StatusCodes.Status201Created, criado.StatusCode);
            Assert.Same(resultado, criado.Value);
        }

        [Fact(DisplayName = "Deve retornar 400 quando cadastro falhar")]
        public async Task DeveRetornarBadRequestQuandoCadastroFalhar()
        {
            var resultado = Result<CadastrarMovimentacoesResponse>.Fail("O valor da movimentação deve ser maior que zero.");

            _movimentacaoServiceMock.Setup(x => x.CadastraMovimentacao(_cadastroRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.CriaMovimentacao(_cadastroRequest);

            var requisicaoInvalida = Assert.IsType<BadRequestObjectResult>(resposta);
            Assert.Same(resultado, requisicaoInvalida.Value);
        }

        [Fact(DisplayName = "Deve retornar 401 ao cadastrar com token inválido")]
        public async Task DeveRetornarUnauthorizedAoCadastrarComTokenInvalido()
        {
            DefinirUsuario(new Claim(ClaimTypes.NameIdentifier, "id-invalido"));

            var resposta = await _controller.CriaMovimentacao(_cadastroRequest);

            var naoAutorizado = Assert.IsType<UnauthorizedObjectResult>(resposta);
            Assert.Equal("Token inválido ou incompleto!", naoAutorizado.Value);
            _movimentacaoServiceMock.Verify(x => x.CadastraMovimentacao(It.IsAny<CadastraMovimentacoesRequest>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar 200 ao consultar movimentações")]
        public async Task DeveRetornarOkAoConsultar()
        {
            var resultado = Result<List<ObterMovimentacoesResponse>>.Ok(new List<ObterMovimentacoesResponse>());

            _movimentacaoServiceMock.Setup(x => x.ObterMovimentacoes(_consultaRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.ObtemMovimentacoes(_consultaRequest);

            var ok = Assert.IsType<OkObjectResult>(resposta);
            Assert.Same(resultado, ok.Value);
        }

        [Fact(DisplayName = "Deve retornar 400 quando consulta falhar")]
        public async Task DeveRetornarBadRequestQuandoConsultaFalhar()
        {
            var resultado = Result<List<ObterMovimentacoesResponse>>.Fail("A data inicial não pode ser maior que a data final.");

            _movimentacaoServiceMock.Setup(x => x.ObterMovimentacoes(_consultaRequest, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.ObtemMovimentacoes(_consultaRequest);

            var requisicaoInvalida = Assert.IsType<BadRequestObjectResult>(resposta);
            Assert.Same(resultado, requisicaoInvalida.Value);
        }

        [Fact(DisplayName = "Deve retornar 401 ao consultar sem a claim do usuário")]
        public async Task DeveRetornarUnauthorizedAoConsultarSemClaim()
        {
            DefinirUsuario();

            var resposta = await _controller.ObtemMovimentacoes(_consultaRequest);

            var naoAutorizado = Assert.IsType<UnauthorizedObjectResult>(resposta);
            Assert.Equal("Token inválido ou incompleto!", naoAutorizado.Value);
            _movimentacaoServiceMock.Verify(x => x.ObterMovimentacoes(It.IsAny<ObterMovimentacoesRequest>(), It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar 204 ao excluir movimentação")]
        public async Task DeveRetornarNoContentAoExcluir()
        {
            var idMovimentacao = Guid.NewGuid();

            _movimentacaoServiceMock.Setup(x => x.DeletaMovimentacao(idMovimentacao, _idUsuario))
                .ReturnsAsync(Result<string>.Ok("Movimentação excluída com sucesso!"));

            var resposta = await _controller.DeletaMovimentacao(idMovimentacao);

            Assert.IsType<NoContentResult>(resposta);
        }

        [Fact(DisplayName = "Deve retornar 404 quando movimentação não for encontrada")]
        public async Task DeveRetornarNotFoundAoExcluirMovimentacaoInexistente()
        {
            var idMovimentacao = Guid.NewGuid();
            var resultado = Result<string>.Fail("Movimentação não encontrada.", ETipoErro.NaoEncontrado);

            _movimentacaoServiceMock.Setup(x => x.DeletaMovimentacao(idMovimentacao, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.DeletaMovimentacao(idMovimentacao);

            var naoEncontrado = Assert.IsType<NotFoundObjectResult>(resposta);
            Assert.Same(resultado, naoEncontrado.Value);
        }

        [Fact(DisplayName = "Deve retornar 401 ao excluir com token inválido")]
        public async Task DeveRetornarUnauthorizedAoExcluirComTokenInvalido()
        {
            DefinirUsuario(new Claim(ClaimTypes.NameIdentifier, "id-invalido"));

            var resposta = await _controller.DeletaMovimentacao(Guid.NewGuid());

            Assert.IsType<UnauthorizedObjectResult>(resposta);
            _movimentacaoServiceMock.Verify(x => x.DeletaMovimentacao(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}

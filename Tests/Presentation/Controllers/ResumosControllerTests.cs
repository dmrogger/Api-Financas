using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Resumos;
using ApiFinancas.Src.Application.DTOs.Responses.Resumos;
using ApiFinancas.Src.Application.Interfaces.Resumos;
using ApiFinancas.Src.Presentation.Controllers.Resumos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace ApiFinancas.Tests.Presentation.Controllers
{
    public class ResumosControllerTests
    {
        private readonly Mock<IResumoService> _serviceMock;
        private readonly ResumosController _controller;
        private readonly Guid _idUsuario = Guid.NewGuid();
        private readonly ObterResumoMensalRequest _request = new ObterResumoMensalRequest { Ano = 2026, Mes = 11 };

        public ResumosControllerTests()
        {
            _serviceMock = new Mock<IResumoService>();
            _controller = new ResumosController(_serviceMock.Object);
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
            var excecao = Assert.Throws<ArgumentNullException>(() => new ResumosController(null!));

            Assert.Equal("resumoService", excecao.ParamName);
        }

        [Fact(DisplayName = "Deve retornar 200 com o resumo mensal")]
        public async Task DeveRetornarOkComResumo()
        {
            var resultado = Result<ResumoMensalResponse>.Ok(new ResumoMensalResponse { Ano = 2026, Mes = 11 });
            _serviceMock.Setup(x => x.ObterResumoMensalAsync(_request, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.ObterResumoMensal(_request);

            var ok = Assert.IsType<OkObjectResult>(resposta);
            Assert.Same(resultado, ok.Value);
        }

        [Fact(DisplayName = "Deve retornar 400 quando o período for inválido")]
        public async Task DeveRetornarBadRequestComPeriodoInvalido()
        {
            var resultado = Result<ResumoMensalResponse>.Fail("Mês deve estar entre 1 e 12.");
            _serviceMock.Setup(x => x.ObterResumoMensalAsync(_request, _idUsuario)).ReturnsAsync(resultado);

            var resposta = await _controller.ObterResumoMensal(_request);

            var invalido = Assert.IsType<BadRequestObjectResult>(resposta);
            Assert.Same(resultado, invalido.Value);
        }

        [Fact(DisplayName = "Deve retornar 401 com token inválido")]
        public async Task DeveRetornarUnauthorizedComTokenInvalido()
        {
            DefinirUsuario(new Claim(ClaimTypes.NameIdentifier, "id-invalido"));

            var resposta = await _controller.ObterResumoMensal(_request);

            Assert.IsType<UnauthorizedObjectResult>(resposta);
            _serviceMock.Verify(x => x.ObterResumoMensalAsync(It.IsAny<ObterResumoMensalRequest>(), It.IsAny<Guid>()), Times.Never);
        }
    }
}

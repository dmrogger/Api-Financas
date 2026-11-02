using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;
using ApiFinancas.Src.Application.Interfaces.Autenticacao;
using ApiFinancas.Src.Presentation.Controllers.Autenticacao;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace ApiFinancas.Tests.Presentation.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAutenticacaoService> _authServiceMock;
        private readonly AuthController _controller;
        private readonly LoginRequest _request;

        public AuthControllerTests()
        {
            _authServiceMock = new Mock<IAutenticacaoService>();
            _controller = new AuthController(_authServiceMock.Object);
            _request = new LoginRequest
            {
                Email = "teste@gmail.com",
                Senha = "Senha@123"
            };
        }

        [Fact(DisplayName = "Deve retornar 200 quando login válido")]
        public async Task DeveRetornarOkQuandoLoginValido()
        {
            var resultado = Result<LoginResponse>.Ok(new LoginResponse(Guid.NewGuid(), "Teste", "teste@gmail.com", "token"));

            _authServiceMock.Setup(x => x.LoginAsync(_request)).ReturnsAsync(resultado);

            var resposta = await _controller.Login(_request);

            var ok = Assert.IsType<OkObjectResult>(resposta);
            Assert.Same(resultado, ok.Value);
        }

        [Fact(DisplayName = "Deve retornar 401 quando login inválido")]
        public async Task DeveRetornarUnauthorizedQuandoLoginInvalido()
        {
            var resultado = Result<LoginResponse>.Fail("Usuário ou senha inválidos");

            _authServiceMock.Setup(x => x.LoginAsync(_request)).ReturnsAsync(resultado);

            var resposta = await _controller.Login(_request);

            var naoAutorizado = Assert.IsType<UnauthorizedObjectResult>(resposta);
            Assert.Same(resultado, naoAutorizado.Value);
        }
    }
}

using ApiFinancas.Src.Presentation.Middlewares;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;

namespace ApiFinancas.Tests.Presentation.Middlewares
{
    public class GlobalExceptionHandlerTests
    {
        private readonly Mock<ILogger<GlobalExceptionHandler>> _loggerMock;
        private readonly GlobalExceptionHandler _handler;
        private readonly DefaultHttpContext _httpContext;

        public GlobalExceptionHandlerTests()
        {
            _loggerMock = new Mock<ILogger<GlobalExceptionHandler>>();
            _handler = new GlobalExceptionHandler(_loggerMock.Object);

            _httpContext = new DefaultHttpContext();
            _httpContext.Response.Body = new MemoryStream();
            _httpContext.TraceIdentifier = "trace-teste";
        }

        private async Task<JsonElement> LerRespostaAsync()
        {
            _httpContext.Response.Body.Position = 0;
            using var documento = await JsonDocument.ParseAsync(_httpContext.Response.Body);
            return documento.RootElement.Clone();
        }

        private void VerificarLog(LogLevel nivel, Exception exception)
        {
            _loggerMock.Verify(x => x.Log(
                nivel,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar 400 com a mensagem quando ArgumentException")]
        public async Task DeveRetornar400QuandoArgumentException()
        {
            var exception = new ArgumentException("O valor é inválido.");

            var tratado = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            var resposta = await LerRespostaAsync();

            Assert.True(tratado);
            Assert.Equal(StatusCodes.Status400BadRequest, _httpContext.Response.StatusCode);
            Assert.Equal(400, resposta.GetProperty("status").GetInt32());
            Assert.Equal("Requisição inválida", resposta.GetProperty("title").GetString());
            Assert.Equal("O valor é inválido.", resposta.GetProperty("detail").GetString());
            Assert.Equal("trace-teste", resposta.GetProperty("traceId").GetString());
            VerificarLog(LogLevel.Warning, exception);
        }

        [Fact(DisplayName = "Deve retornar 400 para exceções derivadas de ArgumentException")]
        public async Task DeveRetornar400QuandoExcecaoDerivadaDeArgumentException()
        {
            var exception = new ArgumentNullException("valor");

            await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            Assert.Equal(StatusCodes.Status400BadRequest, _httpContext.Response.StatusCode);
        }

        [Fact(DisplayName = "Deve retornar 500 sem expor detalhes quando outra exceção")]
        public async Task DeveRetornar500SemExporDetalhes()
        {
            var exception = new InvalidOperationException("Senha do banco: 123");

            var tratado = await _handler.TryHandleAsync(_httpContext, exception, CancellationToken.None);

            var resposta = await LerRespostaAsync();

            Assert.True(tratado);
            Assert.Equal(StatusCodes.Status500InternalServerError, _httpContext.Response.StatusCode);
            Assert.Equal(500, resposta.GetProperty("status").GetInt32());
            Assert.Equal("Erro interno no servidor", resposta.GetProperty("title").GetString());
            Assert.Equal("Ocorreu um erro inesperado. Tente novamente mais tarde.", resposta.GetProperty("detail").GetString());
            Assert.Equal("trace-teste", resposta.GetProperty("traceId").GetString());
            VerificarLog(LogLevel.Error, exception);
        }
    }
}

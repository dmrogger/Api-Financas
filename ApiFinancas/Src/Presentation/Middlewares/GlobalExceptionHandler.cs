using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinancas.Src.Presentation.Middlewares
{
    /// <summary>
    /// Converte exceções não tratadas em respostas ProblemDetails.
    /// Regras de domínio violadas (ArgumentException) viram 400; o resto vira 500 sem expor detalhes.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problem = exception switch
            {
                ArgumentException ex => new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Requisição inválida",
                    Detail = ex.Message
                },
                _ => new ProblemDetails
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Erro interno no servidor",
                    Detail = "Ocorreu um erro inesperado. Tente novamente mais tarde."
                }
            };

            if (problem.Status == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Erro não tratado");
            else
                _logger.LogWarning(exception, "Requisição inválida");

            problem.Extensions["traceId"] = httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = problem.Status!.Value;
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }
    }
}

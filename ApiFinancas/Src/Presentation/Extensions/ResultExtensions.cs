using ApiFinancas.Src.Application.DTOs.Common;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinancas.Src.Presentation.Extensions
{
    public static class ResultExtensions
    {
        /// <summary>
        /// Converte um Result com falha na resposta HTTP correspondente ao tipo do erro
        /// </summary>
        public static IActionResult RespostaDeErro<T>(this ControllerBase controller, Result<T> result)
        {
            return result.TipoErro switch
            {
                ETipoErro.NaoEncontrado => controller.NotFound(result),
                ETipoErro.Proibido => controller.StatusCode(StatusCodes.Status403Forbidden, result),
                ETipoErro.Conflito => controller.Conflict(result),
                _ => controller.BadRequest(result)
            };
        }
    }
}

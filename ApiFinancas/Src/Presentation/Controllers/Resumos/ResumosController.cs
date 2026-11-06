using ApiFinancas.Src.Application.DTOs.Requests.Resumos;
using ApiFinancas.Src.Application.Interfaces.Resumos;
using ApiFinancas.Src.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinancas.Src.Presentation.Controllers.Resumos
{
    /// <summary>
    /// Resumos financeiros do usuário autenticado
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/resumos")]
    [Produces("application/json")]
    public class ResumosController : ControllerBase
    {
        private readonly IResumoService _resumoService;

        public ResumosController(IResumoService resumoService)
        {
            _resumoService = resumoService ?? throw new ArgumentNullException(nameof(resumoService));
        }

        /// <summary>
        /// Totais de entradas, saídas e saldo do mês, agrupados por categoria
        /// </summary>
        [HttpGet("mensal")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObterResumoMensal([FromQuery] ObterResumoMensalRequest request)
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _resumoService.ObterResumoMensalAsync(request, idUsuario);

            if (result.Success)
                return Ok(result);

            return this.RespostaDeErro(result);
        }
    }
}

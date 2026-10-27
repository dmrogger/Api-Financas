using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.Interfaces.Movimentacoes;
using ApiFinancas.Src.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinancas.Src.Presentation.Controllers.Movimentacoes
{
    /// <summary>
    /// Controller para gerenciamento de movimentações financeiras
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/movimentacoes")]
    [Produces("application/json")]
    public class MovimentacoesController : ControllerBase
    {
        private readonly IMovimentacaoService _movimentacaoService;

        /// <summary>
        /// Controller responsável pelas movimentações
        /// </summary>
        /// <param name="movimentacaoService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public MovimentacoesController(IMovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService ?? throw new ArgumentNullException(nameof(movimentacaoService));
        }

        /// <summary>
        /// Cria uma movimentação do usuário autenticado
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CriaMovimentacao([FromBody] CadastraMovimentacoesRequest request)
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _movimentacaoService.CadastraMovimentacao(request, idUsuario);

            if (result.Success)
                return StatusCode(StatusCodes.Status201Created, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Obtém as movimentações do usuário autenticado, com filtro opcional por período
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ObtemMovimentacoes([FromQuery] ObterMovimentacoesRequest request)
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _movimentacaoService.ObterMovimentacoes(request, idUsuario);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}

using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes;
using ApiFinancas.Src.Application.Interfaces.Movimentacoes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiFinancas.Src.Presentation.Controllers.Movimentacoes
{
    /// <summary>
    /// Controller para gerenciamento de movimentações financeiras
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
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
        /// Cria uma nova movimentação
        /// </summary>
        [HttpPost("movimentacao")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CriaMovimentacao([FromBody]  CadastraMovimentacoesRequest request,
                                                      CancellationToken cancellationToken)
        {
            var result = await _movimentacaoService.CadastraMovimentacao(request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        [HttpGet("movimentacoes")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ObtemMovimentacoes([FromBody] ObterMovimentacoesRequest request, CancellationToken cancellationToken)
        {
            var result = await _movimentacaoService.ObterMovimentacoes(request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}

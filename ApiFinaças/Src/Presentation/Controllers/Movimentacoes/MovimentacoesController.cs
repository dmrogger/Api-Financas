using ApiFinaças.Src.Application.DTOs.Requests;
using ApiFinaças.Src.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinaças.Src.Presentation.Controllers.Movimentacoes
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

        public MovimentacoesController(IMovimentacaoService movimentacaoService)
        {
            _movimentacaoService = movimentacaoService ?? throw new ArgumentNullException(nameof(movimentacaoService));
        }

        /// <summary>
        /// Adiciona uma operação de entrada
        /// </summary>
        /// <param name="request">Dados da entrada</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("AdicionarEntrada")]
        [ProducesResponseType(typeof(ApiFinaças.Src.Application.DTOs.Responses.AdicionarEntradaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdicionarEntrada([FromBody] AdicionarEntradaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _movimentacaoService.CriarEntradaAsync(request);

            if (!response.Sucesso)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Adiciona uma operação de saída
        /// </summary>
        /// <param name="request">Dados da saída</param>
        /// <returns>Resultado da operação</returns>
        [HttpPost("AdicionarSaida")]
        [ProducesResponseType(typeof(ApiFinaças.Src.Application.DTOs.Responses.AdicionarSaidaResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AdicionarSaida([FromBody] AdicionarSaidaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _movimentacaoService.CriarSaidaAsync(request);

            if (!response.Sucesso)
                return BadRequest(response);

            return Ok(response);
        }

        /// <summary>
        /// Retorna todas as movimentações cadastradas. O Endpoint pode ser parametrizado 
        /// para filtrá-las com base nos parâmetros fornecidos
        /// </summary>
        /// <param name="usuarioId">ID do usuário</param>
        /// <param name="dataInicial">Data inicial para filtro (opcional)</param>
        /// <param name="dataFinal">Data final para filtro (opcional)</param>
        /// <returns>Lista de movimentações</returns>
        [HttpGet("ObterMovimentações")]
        [ProducesResponseType(typeof(IEnumerable<ApiFinaças.Src.Application.DTOs.Responses.ObterMovimentacoesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ObterMovimentacoes(
            [FromQuery] Guid usuarioId,
            [FromQuery] DateTime? dataInicial = null,
            [FromQuery] DateTime? dataFinal = null)
        {
            if (usuarioId == Guid.Empty)
                return BadRequest("O ID do usuário é obrigatório.");

            var movimentacoes = await _movimentacaoService.ObterMovimentacoesAsync(usuarioId, dataInicial, dataFinal);
            
            return Ok(movimentacoes);
        }

        /// <summary>
        /// Deleta uma operação
        /// </summary>
        /// <param name="movimentacaoId">ID da movimentação</param>
        /// <param name="usuarioId">ID do usuário (para validação)</param>
        /// <returns>Resultado da operação</returns>
        [HttpDelete("ExcluirOperacao")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ExcluirOperacao([FromQuery] Guid movimentacaoId, [FromQuery] Guid usuarioId)
        {
            if (movimentacaoId == Guid.Empty)
                return BadRequest("O ID da movimentação é obrigatório.");

            if (usuarioId == Guid.Empty)
                return BadRequest("O ID do usuário é obrigatório.");

            var sucesso = await _movimentacaoService.DeletarMovimentacaoAsync(movimentacaoId, usuarioId);

            if (!sucesso)
                return NotFound("Movimentação não encontrada ou não pertence ao usuário informado.");

            return Ok(new { Sucesso = true, Mensagem = "Movimentação excluída com sucesso." });
        }
    }
}

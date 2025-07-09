using System.Reflection.Metadata.Ecma335;
using ApiFinaças.Src.Application.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinaças.Src.Presentation.Controllers.Movimentacoes
{
    [ApiController]
   [Route("api/[controller]")]
    public class MovimentacoesController : ControllerBase
    {
        /// <remarks>
        ///  Adiciona uma operacao de entrada 
        /// </remarks>
        [HttpPost("AdicionarEntrada")]
        public async Task<IActionResult> AdicionarEntrada([FromBody] AdicionarEntradaRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return null;
        }

        /// <remarks>
        ///  Adiciona uma operacao de saida 
        /// </remarks>
        [HttpPost("AdicionarSaida")]
        public async Task<IActionResult> AdicionarSaida([FromBody] AdicionarSaidaRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return null;
        }

        /// <remarks>
        ///  Retorna todas as movimentações cadastradas. O Endpoint pode ser parametrizado 
        ///  para filtrá-las com base nos parâmetros fornecidos
        /// </remarks>
        [HttpGet("ObterMovimentações")]
        public Task<IActionResult> ObterEntradas(string filtro)
        {
            if (String.IsNullOrEmpty(filtro))
                return null;

            return null;
        }


        /// <remarks>
        ///  Deleta uma operacao  
        /// </remarks>
        [HttpDelete("ExcluirOperacao")]
        public Task<IActionResult> ExcluirOperacao()
        {
            return null;
        }
    }
}

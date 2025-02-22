using System.Reflection.Metadata.Ecma335;
using ApiFinaças.Src.Application.DTOs.Requests;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinaças.Src.Presentation.Controllers.Movimentacoes
{
    [ApiController]
   // [Route("/entrada")]
    public class MovimentacoesController : ControllerBase
    {
        /// <remarks>
        ///  Adiciona uma operacao de entrada 
        /// </remarks>
        [HttpPost("AdicionarEntrada")]
        public async Task<IActionResult> AdicionarEntrada([FromBody] AdicionarEntradaRequest request)
        {

            return null;
        }

        /// <remarks>
        ///  Adiciona uma operacao de saida 
        /// </remarks>
        [HttpPost("AdicionarSaida")]
        public async Task<IActionResult> AdicionarSaida([FromBody] AdicionarSaidaRequest request)
        {

            return null;
        }

        /// <remarks>
        ///  Retorna todas as operacoes cadastradas. O Endpoint pode ser parametrizado 
        ///  para filtrá-las com base nos parâmetros fornecidos
        /// </remarks>
        [HttpGet("ObterOperacoes")]
        public Task<IActionResult> ObterEntradas(string filtro)
        {
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

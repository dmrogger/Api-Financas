using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinaças.Controllers.Movimentacoes
{
    [ApiController]
    [Route("/entrada")]
    public class EntradasController : ControllerBase
    {
        /// <remarks>
        ///  Retorna todas as entradas cadastradas. O Endpoint pode ser parametrizado 
        ///  para filtrá-las com base nos parâmetros fornecidos
        /// </remarks>
        [HttpGet]
        public Task<IActionResult> ObterEntradas(string filtro)
        {
            return null;
        }

        /// <remarks>
        ///  Adiciona uma entrada 
        /// </remarks>
        [HttpPost]
        public Task<IActionResult> AdicionarEntrada()
        {
            return null;
        }

        /// <remarks>
        ///  Deleta uma entrada  
        /// </remarks>
        [HttpDelete]
        public Task<IActionResult> ExcluirEntrada()
        {
            return null;
        }
    }
}

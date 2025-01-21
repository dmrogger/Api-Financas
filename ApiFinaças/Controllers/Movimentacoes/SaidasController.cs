using Microsoft.AspNetCore.Mvc;

namespace ApiFinaças.Controllers.Movimentacoes
{
    [ApiController]
    [Route("/saida")]
    public class SaidasController : ControllerBase
    {
        [HttpGet]
        public Task<IActionResult> ObterSaidas()
        {
            return null;
        }

        [HttpPost]
        public Task<IActionResult> AdicionarSaidas()
        {
            return null;
        }

        [HttpDelete]
        public Task<IActionResult> ExcluirSaidas()
        {
            return null;
        }
    }
}

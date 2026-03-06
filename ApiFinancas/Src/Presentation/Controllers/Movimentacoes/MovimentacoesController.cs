using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes;
using ApiFinancas.Src.Application.Interfaces.Movimentacoes;
using Microsoft.AspNetCore.Mvc;

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


    }
}

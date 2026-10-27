using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.Interfaces.Autenticacao;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinancas.Src.Presentation.Controllers.Autenticacao
{
    [ApiController]
    [Route("api/v1/auth")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        private readonly IAutenticacaoService _authService;

        public AuthController(IAutenticacaoService authService)
        {
            _authService = authService;
        }

        /// <summary>
        /// Autentica o usuário e retorna o token JWT
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (result.Success)
                return Ok(result);

            return Unauthorized(result);
        }
    }
}

using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.Interfaces.Autenticacao;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinancas.Src.Presentation.Controllers.Autenticacao
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAutenticacaoService _authService;

        public AuthController(IAutenticacaoService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
    }
}

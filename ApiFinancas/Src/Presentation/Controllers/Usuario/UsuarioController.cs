using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.Interfaces.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ApiFinaças.Src.Presentation.Controllers.Usuario
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuariosController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        [HttpPost("usuario")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CriarUsuario([FromBody]CriaUsuarioRequest request, 
                                                      CancellationToken cancellationToken)
        {
           var result = await _usuarioService.CriarUsuarioAsync(request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }


        /// <summary>
        /// Consulta um usuário por E-mail
        /// </summary>
        [Authorize]
        [HttpGet("usuario{email}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ConsultarUsuario()
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;

            var result = await _usuarioService.ConsultaUsuario(email);
            if(result.Success)
                return Ok(result);
            
            return BadRequest(result);
            
   
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        [Authorize]
        [HttpDelete("usuario")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeletarUsuario([FromBody]ExcluiUsuarioRequest request)
        {
            var result = await _usuarioService.DeletaUsuario(request);
            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}

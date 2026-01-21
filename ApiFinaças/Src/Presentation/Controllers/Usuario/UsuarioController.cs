using ApiFinancas.Src.Application.DTOs.Requests.Usuario;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ApiFinaças.Src.Presentation.Controllers.Usuario
{
    [ApiController]
    [Route("api/v1/usuarios")]
    [Produces("application/json")]
    public class UsuariosController : ControllerBase
    {
        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        [HttpPost("usuario")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CriarUsuario([FromBody]CriaUsuarioRequest request, 
                                                      CancellationToken cancellationToken)
        {
            return Created(string.Empty, null);
        }

        /// <summary>
        /// Consulta um usuário por id
        /// </summary>
        [HttpGet("usuario{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ConsultarUsuario(Guid id)
        {
            return Ok();
        }

        /// <summary>
        /// Remove um usuário
        /// </summary>
        [HttpDelete("usuario{id:guid}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task<IActionResult> DeletarUsuario(Guid id)
        {
            return NoContent();
        }
    }
}

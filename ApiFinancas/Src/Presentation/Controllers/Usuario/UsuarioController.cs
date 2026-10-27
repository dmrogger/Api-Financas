using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.Interfaces.Usuario;
using ApiFinancas.Src.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinancas.Src.Presentation.Controllers.Usuario
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
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarUsuario([FromBody] CriaUsuarioRequest request)
        {
            var result = await _usuarioService.CriarUsuarioAsync(request);

            if (result.Success)
                return CreatedAtAction(nameof(ConsultarUsuario), null, result);

            return BadRequest(result);
        }

        /// <summary>
        /// Consulta os dados do usuário autenticado
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConsultarUsuario()
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _usuarioService.ConsultaUsuario(idUsuario);

            if (result.Success)
                return Ok(result);

            return NotFound(result);
        }

        /// <summary>
        /// Edita o nome e/ou a senha do usuário autenticado
        /// </summary>
        [Authorize]
        [HttpPatch("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> EditaUsuario([FromBody] EditaUsuarioRequest request)
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _usuarioService.EditaUsuario(request, idUsuario);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }

        /// <summary>
        /// Remove o usuário autenticado (exige confirmação da senha)
        /// </summary>
        [Authorize]
        [HttpDelete("me")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletarUsuario([FromBody] ExcluiUsuarioRequest request)
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _usuarioService.DeletaUsuario(request, idUsuario);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}

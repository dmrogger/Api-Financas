using ApiFinancas.Src.Application.DTOs.Requests.Categorias;
using ApiFinancas.Src.Application.Interfaces.Categorias;
using ApiFinancas.Src.Presentation.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApiFinancas.Src.Presentation.Controllers.Categorias
{
    /// <summary>
    /// Categorias padrão (do sistema) e personalizadas (do usuário autenticado)
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/v1/categorias")]
    [Produces("application/json")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _categoriaService;

        public CategoriasController(ICategoriaService categoriaService)
        {
            _categoriaService = categoriaService ?? throw new ArgumentNullException(nameof(categoriaService));
        }

        /// <summary>
        /// Lista as categorias padrão e as do usuário autenticado
        /// </summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Listar()
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _categoriaService.ListarAsync(idUsuario);

            return Ok(result);
        }

        /// <summary>
        /// Cria uma categoria personalizada
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Criar([FromBody] CategoriaRequest request)
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _categoriaService.CriarAsync(request, idUsuario);

            if (result.Success)
                return StatusCode(StatusCodes.Status201Created, result);

            return this.RespostaDeErro(result);
        }

        /// <summary>
        /// Renomeia uma categoria personalizada
        /// </summary>
        /// <param name="id">ID da categoria</param>
        /// <param name="request">Novo nome</param>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Editar(Guid id, [FromBody] CategoriaRequest request)
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _categoriaService.EditarAsync(id, request, idUsuario);

            if (result.Success)
                return Ok(result);

            return this.RespostaDeErro(result);
        }

        /// <summary>
        /// Exclui uma categoria personalizada. As movimentações dela ficam sem categoria
        /// </summary>
        /// <param name="id">ID da categoria</param>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deletar(Guid id)
        {
            if (!User.TryObterIdUsuario(out var idUsuario))
                return Unauthorized("Token inválido ou incompleto!");

            var result = await _categoriaService.DeletarAsync(id, idUsuario);

            if (result.Success)
                return NoContent();

            return this.RespostaDeErro(result);
        }
    }
}

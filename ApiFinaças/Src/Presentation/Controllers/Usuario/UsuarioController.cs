using Microsoft.AspNetCore.Mvc;

namespace ApiFinaças.Src.Presentation.Controllers.Usuario
{
    public class UsuarioController : ControllerBase
    {
        [HttpPost("Usuario")]
        public async Task<IActionResult> CriarUsuario()
        {
            return null;
        }
      
    }
}

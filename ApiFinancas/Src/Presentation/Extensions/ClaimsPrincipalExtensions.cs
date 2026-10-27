using System.Security.Claims;

namespace ApiFinancas.Src.Presentation.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Obtém o ID do usuário autenticado a partir do token JWT
        /// </summary>
        public static bool TryObterIdUsuario(this ClaimsPrincipal user, out Guid idUsuario)
        {
            var idUsuarioClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(idUsuarioClaim, out idUsuario);
        }
    }
}

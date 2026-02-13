using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Usuario;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;

namespace ApiFinancas.Src.Application.Interfaces.Usuario
{
    public interface IUsuarioService
    {
        Task<Result<UsuarioResponse>> CriarUsuarioAsync(CriaUsuarioRequest request);
        Task<bool> ValidaLogin(LoginRequest request);

    }
}

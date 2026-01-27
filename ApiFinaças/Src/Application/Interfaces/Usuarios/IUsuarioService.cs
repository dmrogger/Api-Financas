using ApiFinancas.Src.Application.DTOs.Requests.Usuario;

namespace ApiFinancas.Src.Application.Interfaces.Usuario
{
    public interface IUsuarioService
    {

        Task<Guid> CriarUsuarioAsync(CriaUsuarioRequest request);
        Task<bool> ValidaLogin(LoginRequest request);

    }
}

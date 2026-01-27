using ApiFinancas.Src.Application.DTOs.Requests.Usuario;
using ApiFinancas.Src.Application.Interfaces.Usuario;

namespace ApiFinancas.Src.Application.Services.Usuarios
{
    public class UsuarioService : IUsuarioService
    {
        public Task<Guid> CriarUsuarioAsync(CriaUsuarioRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ValidaLogin(LoginRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

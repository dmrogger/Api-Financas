using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;

namespace ApiFinancas.Src.Application.Interfaces.Usuario
{
    public interface IUsuarioService
    {
        Task<Result<LoginResponse>> ConsultaUsuario(string email);
        Task<Result<string>> EditaUsuario(EditaUsuarioRequest request);
        Task<Result<string>> DeletaUsuario(ExcluiUsuarioRequest request);
        Task<Result<UsuarioResponse>> CriarUsuarioAsync(CriaUsuarioRequest request);
    }
}

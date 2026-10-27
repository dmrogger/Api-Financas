using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;

namespace ApiFinancas.Src.Application.Interfaces.Usuario
{
    public interface IUsuarioService
    {
        Task<Result<UsuarioResponse>> ConsultaUsuario(Guid idUsuario);
        Task<Result<string>> EditaUsuario(EditaUsuarioRequest request, Guid idUsuario);
        Task<Result<string>> DeletaUsuario(ExcluiUsuarioRequest request, Guid idUsuario);
        Task<Result<UsuarioResponse>> CriarUsuarioAsync(CriaUsuarioRequest request);
    }
}

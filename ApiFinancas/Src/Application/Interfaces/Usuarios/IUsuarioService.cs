using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Usuario;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;
using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Src.Application.Interfaces.Usuario
{
    public interface IUsuarioService
    {
        Task<Result<UsuarioResponse>> CriarUsuarioAsync(CriaUsuarioRequest request);
        Task<bool> ValidaLogin(LoginRequest request);
        Task<Result<UsuarioResponse>> ConsultaUsuario(string email);
        Task<Result<string>> AtualizaSenha(EditaUsuarioRequest request);
        Task<Result<string>> DeletaUsuario(ExcluiUsuarioRequest request);
    }
}

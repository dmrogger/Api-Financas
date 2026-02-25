using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Application.DTOs.Responses.Usuario;

namespace ApiFinancas.Src.Application.Interfaces.Autenticacao
{
    public interface IAutenticacaoService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
    }
}


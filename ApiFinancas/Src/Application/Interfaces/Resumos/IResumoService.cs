using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Resumos;
using ApiFinancas.Src.Application.DTOs.Responses.Resumos;

namespace ApiFinancas.Src.Application.Interfaces.Resumos
{
    /// <summary>
    /// Interface do serviço de resumos financeiros
    /// </summary>
    public interface IResumoService
    {
        Task<Result<ResumoMensalResponse>> ObterResumoMensalAsync(ObterResumoMensalRequest request, Guid idUsuario);
    }
}

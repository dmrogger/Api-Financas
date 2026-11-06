using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Categorias;
using ApiFinancas.Src.Application.DTOs.Responses.Categorias;

namespace ApiFinancas.Src.Application.Interfaces.Categorias
{
    /// <summary>
    /// Interface do serviço de categorias
    /// </summary>
    public interface ICategoriaService
    {
        Task<Result<List<CategoriaResponse>>> ListarAsync(Guid idUsuario);
        Task<Result<CategoriaResponse>> CriarAsync(CategoriaRequest request, Guid idUsuario);
        Task<Result<CategoriaResponse>> EditarAsync(Guid idCategoria, CategoriaRequest request, Guid idUsuario);
        Task<Result<string>> DeletarAsync(Guid idCategoria, Guid idUsuario);
    }
}

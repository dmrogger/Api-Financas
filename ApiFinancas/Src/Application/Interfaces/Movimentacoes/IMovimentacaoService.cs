using ApiFinancas.Src.Application.DTOs.Common;
using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes;

namespace ApiFinancas.Src.Application.Interfaces.Movimentacoes
{
    /// <summary>
    /// Interface do serviço de movimentações financeiras
    /// </summary>
    public interface IMovimentacaoService
    {
        Task<Result<CadastrarMovimentacoesResponse>> CadastraMovimentacao(CadastraMovimentacoesRequest movimentacaoRequest, Guid idUsuario);
        Task<Result<List<ObterMovimentacoesResponse>>> ObterMovimentacoes(ObterMovimentacoesRequest obterMovimentacoesRequest, Guid idUsuario);
        /// <summary>
        /// Exclui uma movimentação, desde que pertença ao usuário informado
        /// </summary>
        Task<Result<string>> DeletaMovimentacao(Guid idMovimentacao, Guid idUsuario);
    }
}

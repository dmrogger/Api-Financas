using ApiFinancas.Src.Application.DTOs.Requests.Movimentacoes;
using ApiFinancas.Src.Application.DTOs.Responses.Movimentacoes;

namespace ApiFinancas.Src.Application.Interfaces.Movimentacoes
{
    /// <summary>
    /// Interface do serviço de movimentações financeiras
    /// </summary>
    public interface IMovimentacaoService
    {
        /// <summary>
        /// Cria uma nova movimentação de entrada
        /// </summary>
        Task<AdicionarEntradaResponse> CriarEntradaAsync(AdicionarEntradaRequest request);

        /// <summary>
        /// Cria uma nova movimentação de saída
        /// </summary>
        Task<AdicionarSaidaResponse> CriarSaidaAsync(AdicionarSaidaRequest request);

        /// <summary>
        /// Obtém movimentações de um usuário
        /// </summary>
        Task<IEnumerable<ObterMovimentacoesResponse>> ObterMovimentacoesAsync(Guid usuarioId, DateTime? dataInicial = null, DateTime? dataFinal = null);

        /// <summary>
        /// Deleta uma movimentação
        /// </summary>
        Task<bool> DeletarMovimentacaoAsync(Guid movimentacaoId, Guid usuarioId);
    }
}

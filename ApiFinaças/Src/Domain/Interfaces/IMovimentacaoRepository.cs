using ApiFinancas.Src.Domain.Entities;   

namespace ApiFinancas.Src.Domain.Interfaces
{
    /// <summary>
    /// Interface do repositório de movimentações financeiras
    /// </summary>
    public interface IMovimentacaoRepository
    {
        /// <summary>
        /// Cria uma nova movimentação
        /// </summary>
        Task<Movimentacao> CriarAsync(Movimentacao movimentacao);

        /// <summary>
        /// Obtém uma movimentação pelo ID
        /// </summary>
        Task<Movimentacao?> ObterPorIdAsync(Guid id);

        /// <summary>
        /// Obtém todas as movimentações de um usuário
        /// </summary>
        Task<IEnumerable<Movimentacao>> ObterPorUsuarioAsync(Guid usuarioId);

        /// <summary>
        /// Obtém movimentações de um usuário com filtros opcionais
        /// </summary>
        Task<IEnumerable<Movimentacao>> ObterPorUsuarioComFiltrosAsync(
            Guid usuarioId, 
            DateTime? dataInicial = null, 
            DateTime? dataFinal = null);

        /// <summary>
        /// Atualiza uma movimentação existente
        /// </summary>
        Task<Movimentacao> AtualizarAsync(Movimentacao movimentacao);

        /// <summary>
        /// Deleta uma movimentação
        /// </summary>
        Task<bool> DeletarAsync(Guid id);
    }
}

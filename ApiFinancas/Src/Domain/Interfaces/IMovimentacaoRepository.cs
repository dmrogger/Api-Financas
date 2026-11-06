using ApiFinancas.Src.Domain.Consultas;
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
        Task<Guid> CriarAsync(Movimentacao movimentacao);

        /// <summary>
        /// Obtém uma movimentação pelo ID
        /// </summary>
        Task<Movimentacao?> ObterPorIdAsync(Guid id);

        /// <summary>
        /// Obtém movimentações de um usuário com filtros opcionais
        /// </summary>
        Task<List<Movimentacao>> ObterPorUsuarioComFiltrosAsync(
            Guid usuarioId, 
            DateTime? dataInicial = null, 
            DateTime? dataFinal = null);

        /// <summary>
        /// Deleta uma movimentação
        /// </summary>
        Task DeletarAsync(Movimentacao movimentacao);

        /// <summary>
        /// Soma entradas e saídas do usuário no período [inicio, fim), agrupando por categoria.
        /// A agregação é feita no banco
        /// </summary>
        Task<List<TotalPorCategoria>> ObterTotaisPorCategoriaAsync(Guid usuarioId, DateTime inicioUtc, DateTime fimUtcExclusivo);
    }
}

using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Src.Domain.Interfaces
{
    /// <summary>
    /// Interface do repositório de categorias
    /// </summary>
    public interface ICategoriaRepository
    {
        Task<Categoria?> ObterPorIdAsync(Guid id);

        /// <summary>
        /// Lista as categorias padrão e as do usuário, ordenadas por nome
        /// </summary>
        Task<List<Categoria>> ListarVisiveisParaUsuarioAsync(Guid usuarioId);

        /// <summary>
        /// Indica se já existe, entre as categorias visíveis ao usuário, uma com o mesmo nome
        /// (sem diferenciar maiúsculas e minúsculas). Use idIgnorado para desconsiderar a própria categoria numa edição
        /// </summary>
        Task<bool> ExisteNomeVisivelParaUsuarioAsync(Guid usuarioId, string nome, Guid? idIgnorado = null);

        Task AdicionarAsync(Categoria categoria);
        Task AtualizarAsync(Categoria categoria);
        Task DeletarAsync(Categoria categoria);
    }
}

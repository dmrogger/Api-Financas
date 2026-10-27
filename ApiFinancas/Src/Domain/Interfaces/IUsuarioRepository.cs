using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Src.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync(Guid id);
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Guid> AdicionarAsync(Usuario usuario);
        Task EditaUsuarioAsync(Usuario usuario);
        Task DeletarAsync(Usuario usuario);
    }
}

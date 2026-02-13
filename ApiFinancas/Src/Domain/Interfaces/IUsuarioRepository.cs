using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Src.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorIdAsync (string id);
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Guid> AdicionarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
    }
}

using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Src.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario?> ObterPorIdAsync (string id);
        Task<bool> ExisteEmailAsync(string email);
        Task AdicionarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
    }
}

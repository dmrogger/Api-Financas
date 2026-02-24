using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Src.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Guid> AdicionarAsync(Usuario usuario);
        Task AtualizarAsync(Usuario usuario);
        Task DeletarAsync(Usuario usuario);
    }
}

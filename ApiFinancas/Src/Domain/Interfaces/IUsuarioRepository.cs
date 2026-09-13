using ApiFinancas.Src.Application.DTOs.Autenticacao;
using ApiFinancas.Src.Domain.Entities;

namespace ApiFinancas.Src.Domain.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Guid> AdicionarAsync(Usuario usuario);
        Task EditaUsuarioAsync (Usuario usuario);
        Task AtualizarSenhaAsync(Usuario usuario);
        Task DeletarAsync(Usuario usuario);
    }
}

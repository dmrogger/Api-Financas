using ApiFinaças.Src.Domain.Entities;

namespace ApiFinaças.Src.Infrastructure.Repositories.Interfaces
{
    public interface IMovimentacaoRepository
    {
        Task<IEnumerable<Movimentacao>> ObterUsuarioPeloId(Guid usuarioId);
        Task CriarMovimentacao(Movimentacao movimentacao);
    }
}

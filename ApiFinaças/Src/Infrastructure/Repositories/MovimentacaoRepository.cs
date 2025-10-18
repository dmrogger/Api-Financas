using ApiFinaças.Src.Domain.Entities;

namespace ApiFinaças.Src.Infrastructure.Repositories
{
    public class MovimentacaoRepository
    {
        private readonly List<Movimentacao> _movimentacoes = new();

        public Task CriarMovimentacao(Movimentacao movimentacao)
        {
            _movimentacoes.Add(movimentacao);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<Movimentacao>> ObterUsuarioPeloId(Guid usuarioId)
        {
            return Task.FromResult(_movimentacoes.Where(m => m.UsuarioId == usuarioId).AsEnumerable());
        }

   
    }
}

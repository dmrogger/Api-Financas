using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;

namespace ApiFinaças.Src.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de movimentações (em memória)
    /// </summary>
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        private readonly List<Movimentacao> _movimentacoes = new();

        public Task<Movimentacao> CriarAsync(Movimentacao movimentacao)
        {
            _movimentacoes.Add(movimentacao);
            return Task.FromResult(movimentacao);
        }

        public Task<Movimentacao?> ObterPorIdAsync(Guid id)
        {
            var movimentacao = _movimentacoes.FirstOrDefault(m => m.Id == id);
            return Task.FromResult(movimentacao);
        }

        public Task<IEnumerable<Movimentacao>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            var movimentacoes = _movimentacoes.Where(m => m.UsuarioId == usuarioId).ToList();
            return Task.FromResult<IEnumerable<Movimentacao>>(movimentacoes);
        }

        public Task<IEnumerable<Movimentacao>> ObterPorUsuarioComFiltrosAsync(
            Guid usuarioId, 
            DateTime? dataInicial = null, 
            DateTime? dataFinal = null)
        {
            var query = _movimentacoes.Where(m => m.UsuarioId == usuarioId);

            if (dataInicial.HasValue)
                query = query.Where(m => m.Data >= dataInicial.Value);

            if (dataFinal.HasValue)
                query = query.Where(m => m.Data <= dataFinal.Value);

            return Task.FromResult<IEnumerable<Movimentacao>>(query.ToList());
        }

        public Task<Movimentacao> AtualizarAsync(Movimentacao movimentacao)
        {
            var index = _movimentacoes.FindIndex(m => m.Id == movimentacao.Id);
            if (index >= 0)
            {
                _movimentacoes[index] = movimentacao;
                return Task.FromResult(movimentacao);
            }

            throw new KeyNotFoundException($"Movimentação com ID {movimentacao.Id} não encontrada.");
        }

        public Task<bool> DeletarAsync(Guid id)
        {
            var movimentacao = _movimentacoes.FirstOrDefault(m => m.Id == id);
            if (movimentacao != null)
            {
                _movimentacoes.Remove(movimentacao);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}

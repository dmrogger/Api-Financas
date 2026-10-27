using ApiFinancas.Src.Infrastructure.Persistence;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiFinancas.Src.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de movimentações (PostgreSQL via EF Core)
    /// </summary>
    public class MovimentacaoRepository : IMovimentacaoRepository
    {
        private readonly AppDbContext _context;
        public MovimentacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CriarAsync(Movimentacao movimentacao)
        {
            await _context.Movimentacao.AddAsync(movimentacao);
            await _context.SaveChangesAsync();

            return movimentacao.Id;
        }

        public async Task<Movimentacao?> ObterPorIdAsync(Guid id)
        {
            return await _context.Movimentacao.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<List<Movimentacao>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            return await _context.Movimentacao
                .AsNoTracking()
                .Where(x => x.UsuarioId == usuarioId)
                .OrderByDescending(x => x.Data)
                .ToListAsync();
        }

        public async Task<List<Movimentacao>> ObterPorUsuarioComFiltrosAsync(
            Guid usuarioId,
            DateTime? dataInicial = null,
            DateTime? dataFinal = null)
        {
            var query = _context.Movimentacao
                .AsNoTracking()
                .Where(m => m.UsuarioId == usuarioId);

            if (dataInicial.HasValue)
                query = query.Where(m => m.Data >= dataInicial.Value);

            if (dataFinal.HasValue)
                query = query.Where(m => m.Data <= dataFinal.Value);

            return await query.OrderByDescending(m => m.Data).ToListAsync();
        }

        public async Task<Movimentacao> AtualizarAsync(Movimentacao movimentacao)
        {
            _context.Movimentacao.Update(movimentacao);
            await _context.SaveChangesAsync();

            return movimentacao;
        }

        public async Task<bool> DeletarAsync(Guid id)
        {
            var movimentacao = await _context.Movimentacao.FirstOrDefaultAsync(m => m.Id == id);
            if (movimentacao == null)
                return false;

            _context.Movimentacao.Remove(movimentacao);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}

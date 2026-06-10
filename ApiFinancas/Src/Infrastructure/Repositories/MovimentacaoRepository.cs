using ApiFinaças.Src.Infrastructure.Persistence;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design.Serialization;

namespace ApiFinaças.Src.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de movimentações (em memória)
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
            var movimentacao = await _context.Movimentacao.FirstOrDefaultAsync(x => x.Id == id);
            
            return movimentacao;
        }

        public Task<List<Movimentacao>> ObterPorUsuarioAsync(Guid usuarioId)
        {
            var movimentacoes = _context.Movimentacao
                .Where(x => x.UsuarioId == usuarioId)
                .ToListAsync();
            
            return movimentacoes;
        }

        public Task<List<Movimentacao>> ObterPorUsuarioComFiltrosAsync(
            Guid usuarioId, 
            DateTime? dataInicial = null, 
            DateTime? dataFinal = null)
        {
            var query = _context.Movimentacao.Where(m => m.UsuarioId == usuarioId);

            if (dataInicial.HasValue)
                query = query.Where(m => m.Data >= dataInicial.Value);

            if (dataFinal.HasValue)
                query = query.Where(m => m.Data <= dataFinal.Value);

            return query.ToListAsync();
        }

        public async Task<Movimentacao> AtualizarAsync(Movimentacao movimentacao)
        {
            _context.Movimentacao.Update(movimentacao);

            await _context.SaveChangesAsync();  

            throw new KeyNotFoundException($"Movimentação com ID {movimentacao.Id} não encontrada.");
        }

        public Task<bool> DeletarAsync(Guid id)
        {
            var movimentacao = _context.Movimentacao.FirstOrDefault(m => m.Id == id);
            if (movimentacao != null)
            {
                _context.Movimentacao.Remove(movimentacao);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}

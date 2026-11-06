using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using ApiFinancas.Src.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApiFinancas.Src.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de categorias (PostgreSQL via EF Core)
    /// </summary>
    public class CategoriaRepository : ICategoriaRepository
    {
        private readonly AppDbContext _context;

        public CategoriaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Categoria?> ObterPorIdAsync(Guid id)
        {
            return await _context.Categorias.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Categoria>> ListarVisiveisParaUsuarioAsync(Guid usuarioId)
        {
            return await _context.Categorias
                .AsNoTracking()
                .Where(c => c.UsuarioId == null || c.UsuarioId == usuarioId)
                .OrderBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<bool> ExisteNomeVisivelParaUsuarioAsync(Guid usuarioId, string nome, Guid? idIgnorado = null)
        {
            var nomeMinusculo = nome.Trim().ToLower();

            return await _context.Categorias
                .Where(c => c.UsuarioId == null || c.UsuarioId == usuarioId)
                .Where(c => idIgnorado == null || c.Id != idIgnorado)
                .AnyAsync(c => c.Nome.ToLower() == nomeMinusculo);
        }

        public async Task AdicionarAsync(Categoria categoria)
        {
            await _context.Categorias.AddAsync(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Categoria categoria)
        {
            _context.Categorias.Update(categoria);
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(Categoria categoria)
        {
            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();
        }
    }
}

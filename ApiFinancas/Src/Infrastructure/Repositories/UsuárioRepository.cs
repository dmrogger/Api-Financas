using ApiFinaças.Src.Infrastructure.Persistence;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApiFinancas.Src.Infrastructure.Repositories
{
    public class UsuárioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;
        public UsuárioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> AdicionarAsync(Usuario usuario)
        {
           await _context.Usuarios.AddAsync(usuario);
           await _context.SaveChangesAsync();

           return usuario.Id;
        }

        public async Task AtualizarAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }

        public Task<Usuario?> ObterPorIdAsync(string id)
        {
            return _context.Usuarios.FirstOrDefaultAsync(u => u.Id.ToString() == id);
        }
    }
}

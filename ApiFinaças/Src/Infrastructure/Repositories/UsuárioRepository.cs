using ApiFinaças.Src.Infrastructure.Persistence;
using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Interfaces;

namespace ApiFinancas.Src.Infrastructure.Repositories
{
    public class UsuárioRepository : IUsuarioRepository
    {

        public UsuárioRepository(AppDbContext context)
        {
          _contex = context;
        }

        public async Task AdicionarAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task AtualizarAsync(Usuario usuario)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExisteEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario?> ObterPorEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Usuario?> ObterPorIdAsync(string id)
        {
            throw new NotImplementedException();
        }
    }
}

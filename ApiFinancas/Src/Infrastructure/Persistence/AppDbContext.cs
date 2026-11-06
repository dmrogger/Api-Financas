using ApiFinancas.Src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiFinancas.Src.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Movimentacao> Movimentacao { get; set; }

        // Conta e TipoConta ainda não têm mapeamento nem endpoints;
        // entram no DbContext (e numa migration) quando a funcionalidade for implementada.

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}

using ApiFinacas.Src.Domain.Entities;
using ApiFinancas.Src.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApiFinaças.Src.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Conta> Contas { get; set; }
        public DbSet<TipoConta> TiposConta { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}

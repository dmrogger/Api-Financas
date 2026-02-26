using ApiFinancas.Src.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiFinancas.Src.Infrastructure.Persistence
{
    public class UsuarioConfigurations : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("user", "finances");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id");

            builder.Property(u => u.Nome)
                .HasColumnName("name");

            builder.Property(u => u.Email)
                .HasColumnName("email");

            builder.Property(u => u.Senha)
                .HasColumnName("password_hash");
        }
    }
}

using ApiFinancas.Src.Domain.Entities;
using ApiFinancas.Src.Infrastructure.Persistence.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiFinancas.Src.Infrastructure.Persistence
{
    public class CategoriaConfigurations : IEntityTypeConfiguration<Categoria>
    {
        public void Configure(EntityTypeBuilder<Categoria> builder)
        {
            builder.ToTable("categories", "finances");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(c => c.Nome)
                .HasColumnName("name")
                .HasMaxLength(Categoria.TamanhoMaximoNome)
                .IsRequired();

            // Nulo = categoria padrão
            builder.Property(c => c.UsuarioId)
                .HasColumnName("user_id");

            // Excluir o usuário remove as categorias personalizadas dele
            builder.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            // Mesmo nome não se repete para o mesmo usuário.
            // A regra sem diferenciar maiúsculas/minúsculas e contra as categorias padrão fica no service
            builder.HasIndex(c => new { c.UsuarioId, c.Nome })
                .IsUnique();

            builder.HasData(CategoriasPadrao.Todas());
        }
    }
}

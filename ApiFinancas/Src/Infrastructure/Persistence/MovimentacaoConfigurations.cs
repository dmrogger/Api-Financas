using ApiFinancas.Src.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApiFinancas.Src.Infrastructure.Persistence
{
    public class MovimentacaoConfigurations : IEntityTypeConfiguration<Movimentacao>
    {
        public void Configure(EntityTypeBuilder<Movimentacao> builder)
        {
            builder.ToTable("transactions", "finances");
            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .HasColumnName("id");

            builder.Property(u => u.UsuarioId)
                .HasColumnName("user_id")
                .IsRequired();

            builder.Property(u => u.CategoriaId)
                .HasColumnName("category_id");

            builder.Property(u => u.Valor)
                .HasColumnName("amount")
                .HasPrecision(18, 2);

            builder.Property(u => u.Data)
                .HasColumnName("transaction_date");

            builder.Property(u => u.Tipo)
                .HasColumnName("type")
                .HasConversion<short>()
                .IsRequired();

            builder.Property(u => u.Descricao)
                .HasColumnName("description")
                .HasMaxLength(255);

            builder.HasIndex(u => new { u.UsuarioId, u.Data });
        }
    }
}

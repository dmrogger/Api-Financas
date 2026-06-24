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
                .HasColumnName("user_id");

            builder.Property(u => u.CategoriaId)
                .HasColumnName("category_id");

            builder.Property(u => u.Valor)
                .HasColumnName("amount");

            builder.Property(u => u.Data)
                .HasColumnName("transaction_date");
        }
    }
}

using CustosPE.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustosPE.API.Infrastructure.Data.Configurations;

public class OrgaoConfiguration : IEntityTypeConfiguration<Orgao>
{
    public void Configure(EntityTypeBuilder<Orgao> builder)
    {
        builder.ToTable("orgaos");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(o => o.Codigo)
            .HasColumnName("codigo")
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(o => o.Nome)
            .HasColumnName("nome")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(o => o.Tipo)
            .HasColumnName("tipo")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(o => o.Sigla)
            .HasColumnName("sigla")
            .HasMaxLength(20);

        builder.Property(o => o.CriadoEm)
            .HasColumnName("criado_em")
            .HasDefaultValueSql("NOW()");

        builder.HasIndex(o => o.Codigo).IsUnique();
    }
}

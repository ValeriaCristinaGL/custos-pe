using CustosPE.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustosPE.API.Infrastructure.Data.Configurations;

public class ReceitaConfiguration : IEntityTypeConfiguration<Receita>
{
    public void Configure(EntityTypeBuilder<Receita> builder)
    {
        builder.ToTable("receitas");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(r => r.OrgaoId)
            .HasColumnName("orgao_id")
            .IsRequired();

        builder.Property(r => r.Ano)
            .HasColumnName("ano")
            .IsRequired();

        builder.Property(r => r.Mes)
            .HasColumnName("mes")
            .IsRequired();

        builder.Property(r => r.ValorArrecadado)
            .HasColumnName("valor_arrecadado")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(r => r.ValorPrevisto)
            .HasColumnName("valor_previsto")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(r => r.Categoria)
            .HasColumnName("categoria")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(r => r.Fonte)
            .HasColumnName("fonte")
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(r => r.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(500);

        builder.Property(r => r.CriadoEm)
            .HasColumnName("criado_em")
            .HasDefaultValueSql("NOW()");

        builder.HasOne(r => r.Orgao)
            .WithMany(o => o.Receitas)
            .HasForeignKey(r => r.OrgaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(r => new { r.OrgaoId, r.Ano, r.Mes });
        builder.HasIndex(r => r.Ano);
    }
}

using CustosPE.API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CustosPE.API.Infrastructure.Data.Configurations;

public class DespesaConfiguration : IEntityTypeConfiguration<Despesa>
{
    public void Configure(EntityTypeBuilder<Despesa> builder)
    {
        builder.ToTable("despesas");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(d => d.OrgaoId)
            .HasColumnName("orgao_id")
            .IsRequired();

        builder.Property(d => d.Ano)
            .HasColumnName("ano")
            .IsRequired();

        builder.Property(d => d.Mes)
            .HasColumnName("mes")
            .IsRequired();

        builder.Property(d => d.ValorEmpenhado)
            .HasColumnName("valor_empenhado")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(d => d.ValorLiquidado)
            .HasColumnName("valor_liquidado")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(d => d.ValorPago)
            .HasColumnName("valor_pago")
            .HasColumnType("numeric(18,2)")
            .IsRequired();

        builder.Property(d => d.Categoria)
            .HasColumnName("categoria")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Funcao)
            .HasColumnName("funcao")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Subfuncao)
            .HasColumnName("subfuncao")
            .HasMaxLength(100);

        builder.Property(d => d.Descricao)
            .HasColumnName("descricao")
            .HasMaxLength(500);

        builder.Property(d => d.CriadoEm)
            .HasColumnName("criado_em")
            .HasDefaultValueSql("NOW()");

        builder.HasOne(d => d.Orgao)
            .WithMany(o => o.Despesas)
            .HasForeignKey(d => d.OrgaoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(d => new { d.OrgaoId, d.Ano, d.Mes });
        builder.HasIndex(d => d.Ano);
    }
}

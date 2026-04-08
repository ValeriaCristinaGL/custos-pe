using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TransparenciaPE.Domain.Entities;

namespace TransparenciaPE.Infrastructure.Data.Configurations;

public class EmpenhoConfiguration : IEntityTypeConfiguration<Empenho>
{
    public void Configure(EntityTypeBuilder<Empenho> builder)
    {
        builder.ToTable("empenhos");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.NumeroEmpenho).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Credor).IsRequired().HasMaxLength(200);
        builder.Property(e => e.CnpjCredor).IsRequired().HasMaxLength(14);
        builder.Property(e => e.Valor).HasColumnType("numeric(18,2)");
        builder.Property(e => e.Descricao).HasMaxLength(500);
        builder.Property(e => e.ClassificacaoMcasp).HasMaxLength(100);

        builder.HasIndex(e => e.CnpjCredor).HasDatabaseName("ix_empenhos_cnpj");
        builder.HasIndex(e => e.DataEmpenho).HasDatabaseName("ix_empenhos_data");
        builder.HasIndex(e => new { e.NumeroEmpenho, e.Ano }).IsUnique().HasDatabaseName("ix_empenhos_numero_ano");

        builder.HasOne(e => e.OrgaoGoverno)
            .WithMany(o => o.Empenhos)
            .HasForeignKey(e => e.OrgaoGovernoId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Liquidacoes)
            .WithOne(l => l.Empenho)
            .HasForeignKey(l => l.EmpenhoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class LiquidacaoConfiguration : IEntityTypeConfiguration<Liquidacao>
{
    public void Configure(EntityTypeBuilder<Liquidacao> builder)
    {
        builder.ToTable("liquidacoes");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.NumeroLiquidacao).IsRequired().HasMaxLength(50);
        builder.Property(l => l.Valor).HasColumnType("numeric(18,2)");

        builder.HasMany(l => l.Pagamentos)
            .WithOne(p => p.Liquidacao)
            .HasForeignKey(p => p.LiquidacaoId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PagamentoConfiguration : IEntityTypeConfiguration<Pagamento>
{
    public void Configure(EntityTypeBuilder<Pagamento> builder)
    {
        builder.ToTable("pagamentos");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.NumeroPagamento).IsRequired().HasMaxLength(50);
        builder.Property(p => p.Valor).HasColumnType("numeric(18,2)");
    }
}

public class ContratoConfiguration : IEntityTypeConfiguration<Contrato>
{
    public void Configure(EntityTypeBuilder<Contrato> builder)
    {
        builder.ToTable("contratos");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NumeroContrato).IsRequired().HasMaxLength(50);
        builder.Property(c => c.Fornecedor).IsRequired().HasMaxLength(200);
        builder.Property(c => c.CnpjFornecedor).IsRequired().HasMaxLength(14);
        builder.Property(c => c.ValorContrato).HasColumnType("numeric(18,2)");
        builder.Property(c => c.Objeto).HasMaxLength(1000);

        builder.HasIndex(c => c.CnpjFornecedor).HasDatabaseName("ix_contratos_cnpj");
        builder.HasIndex(c => c.NumeroContrato).IsUnique().HasDatabaseName("ix_contratos_numero");

        builder.HasOne(c => c.OrgaoGoverno)
            .WithMany(o => o.Contratos)
            .HasForeignKey(c => c.OrgaoGovernoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrgaoGovernoConfiguration : IEntityTypeConfiguration<OrgaoGoverno>
{
    public void Configure(EntityTypeBuilder<OrgaoGoverno> builder)
    {
        builder.ToTable("orgaos_governo");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.Codigo).IsRequired().HasMaxLength(20);
        builder.Property(o => o.Nome).IsRequired().HasMaxLength(200);
        builder.Property(o => o.Sigla).HasMaxLength(20);
        builder.Property(o => o.Tipo).HasMaxLength(50);

        builder.HasIndex(o => o.Codigo).IsUnique().HasDatabaseName("ix_orgaos_codigo");
        builder.HasIndex(o => o.Nome).HasDatabaseName("ix_orgaos_nome");
    }
}

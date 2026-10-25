using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockPilot.Domain.Entities;

namespace StockPilot.Infrastructure.Configurations;

public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.ToTable("TB_STOCKPILOT_SUPPLIER");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("SUPPLIER_ID")
            .ValueGeneratedNever();

        builder.Property(s => s.Name)
            .HasColumnName("NAME")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(s => s.Email)
            .HasColumnName("EMAIL")
            .HasMaxLength(200)
            .IsRequired();

        builder.HasIndex(s => s.Email)
            .IsUnique();

        builder.Property(s => s.ZipCode)
            .HasColumnName("ZIP_CODE")
            .HasMaxLength(8);

        builder.Property(s => s.City)
            .HasColumnName("CITY")
            .HasMaxLength(100);

        builder.Property(s => s.State)
            .HasColumnName("STATE")
            .HasMaxLength(2);

        builder.Property(s => s.CreatedAt)
            .HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("UPDATED_AT");
    }
}
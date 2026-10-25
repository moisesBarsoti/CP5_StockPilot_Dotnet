using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StockPilot.Domain.Entities;

namespace StockPilot.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("TB_STOCKPILOT_PRODUCT");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .HasColumnName("PRODUCT_ID")
            .ValueGeneratedNever();

        builder.Property(p => p.Name)
            .HasColumnName("NAME")
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.Price)
            .HasColumnName("PRICE")
            .HasPrecision(12, 2)
            .IsRequired();

        builder.Property(p => p.StockQuantity)
            .HasColumnName("STOCK_QUANTITY")
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .HasColumnName("CREATED_AT")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("UPDATED_AT");

        builder.Property(p => p.SupplierId)
            .HasColumnName("SUPPLIER_ID")
            .IsRequired();

        builder.HasOne(p => p.Supplier)
            .WithMany(s => s.Products)
            .HasForeignKey(p => p.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(p => p.SupplierId);
    }
}
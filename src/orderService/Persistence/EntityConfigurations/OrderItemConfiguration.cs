using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.EntityConfigurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems").HasKey(oi => oi.Id);

        builder.Property(oi => oi.Id).HasColumnName("Id").IsRequired();
        builder.Property(oi => oi.UserId).HasColumnName("UserId");
        builder.Property(oi => oi.ProductId).HasColumnName("ProductId");
        builder.Property(oi => oi.ProductName).HasColumnName("ProductName");
        builder.Property(oi => oi.UnitPrice).HasColumnName("UnitPrice");
        builder.Property(oi => oi.Quantity).HasColumnName("Quantity");
        builder.Property(oi => oi.Address).HasColumnName("Address");
        builder.Property(oi => oi.CreatedDate).HasColumnName("CreatedDate").IsRequired();
        builder.Property(oi => oi.UpdatedDate).HasColumnName("UpdatedDate");
        builder.Property(oi => oi.DeletedDate).HasColumnName("DeletedDate");

        builder.HasQueryFilter(oi => !oi.DeletedDate.HasValue);
    }
}
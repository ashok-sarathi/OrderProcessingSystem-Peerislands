using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingSystem.Data.Entities;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Data.EntityMappings
{
    [ExcludeFromCodeCoverage]
    public class OrderItemMapping : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.Property(c => c.OrderId)
                   .IsRequired();

            builder.Property(c => c.ItemId)
                   .IsRequired();

            builder.Property(c => c.Quantity)
                   .IsRequired();

            builder.Property(c => c.Price)
                   .IsRequired();

            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Order)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(c => c.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(c => c.Item)
                   .WithMany(o => o.OrderItems)
                   .HasForeignKey(c => c.ItemId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.EntityMappings
{
    public class OrderMapping : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(c => c.OrderStatus)
                   .IsRequired();

            builder.Property(c => c.CustomerId)
                   .IsRequired();

            builder.Property(c => c.OrderDate)
                    .IsRequired();

            builder.Property(c => c.LastModifiedDate)
                    .IsRequired();

            builder.HasKey(c => c.Id);

            builder.HasOne(c => c.Customer)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(c => c.CustomerId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

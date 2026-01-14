using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.EntityMappings
{
    public class CustomerMapping : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(c => c.Email)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.Property(c => c.Phone)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.Property(c => c.PermanentAddress)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.Property(c => c.ShippingAddress)
                    .IsRequired()
                    .HasMaxLength(50);

            builder.HasKey(c => c.Id);
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderProcessingSystem.Data.Entities;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Data.EntityMappings
{
    [ExcludeFromCodeCoverage]
    public class ProductMapping : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100);

            builder.Property(c => c.Description)
                    .IsRequired()
                    .HasMaxLength(100);

            builder.Property(c => c.Price)
                    .IsRequired();

            builder.HasKey(c => c.Id);
        }
    }
}

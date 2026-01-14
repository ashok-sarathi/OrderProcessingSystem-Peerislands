using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Data.Contexts
{
    public class OrderProcessingSystemContext(DbContextOptions<OrderProcessingSystemContext> options)
        : DbContext(options)
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrderProcessingSystemContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}

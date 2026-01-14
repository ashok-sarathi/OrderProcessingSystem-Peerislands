using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;

namespace OrderProcessingSystem.Api.Helper.Extentions
{
    public static class ApplicationExtentions
    {
        public static void SeedData(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OrderProcessingSystemContext>();

                context.Customers.AddRange(new[]
                {
                    new Customer
                    {
                        Id = Guid.Parse("10000000-0000-0000-0000-000000000000"),
                        Name = "John Doe",
                        Email = "johndeo@mydomain.com",
                        Phone = "123-456-7890",
                        PermanentAddress = "123 Main St, Hometown, Country",
                        ShippingAddress = "123 Main St, Hometown, Country"
                    },
                    new Customer
                    {
                        Id = Guid.Parse("20000000-0000-0000-0000-000000000000"),
                        Name = "Jane Smith",
                        Email = "JaneSmith@mydomain.com",
                        Phone = "987-654-3210",
                        PermanentAddress = "456 Elm St, Hometown, Country",
                        ShippingAddress = "456 Elm St, Hometown, Country"
                    }
                });

                context.Products.AddRange(new[]
                {
                    new Product
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000000"),
                        Name = "Sample Product 1",
                        Description = "This is a sample product description.",
                        Price = 19.99m
                    },
                    new Product
                    {
                        Id = Guid.Parse("40000000-0000-0000-0000-000000000000"),
                        Name = "Sample Product 2",
                        Description = "This is another sample product description.",
                        Price = 29.99m
                    }
                });

                context.SaveChanges();
            }
        }
    }
}

using OrderProcessingSystem.Data.Contexts;
using OrderProcessingSystem.Data.Entities;
using System.Diagnostics.CodeAnalysis;

namespace OrderProcessingSystem.Api.Helper.Extentions
{
    [ExcludeFromCodeCoverage]
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
                        Name = "Smith Jane",
                        Email = "SmithJane@mydomain.com",
                        Phone = "987-654-3210",
                        PermanentAddress = "456 Elm St, Hometown, Country",
                        ShippingAddress = "456 Elm St, Hometown, Country"
                    },
                    new Customer
                    {
                        Id = Guid.Parse("80000000-0000-0000-0000-000000000000"),
                        Name = "Michael Brown",
                        Email = "michael.brown@mydomain.com",
                        Phone = "555-123-4567",
                        PermanentAddress = "789 Oak Street, Springfield, Country",
                        ShippingAddress = "789 Oak Street, Springfield, Country"
                    },
                    new Customer
                    {
                        Id = Guid.Parse("90000000-0000-0000-0000-000000000000"),
                        Name = "Emily Johnson",
                        Email = "emily.johnson@mydomain.com",
                        Phone = "555-987-6543",
                        PermanentAddress = "12 Pine Avenue, Lakeside, Country",
                        ShippingAddress = "12 Pine Avenue, Lakeside, Country"
                    },
                    new Customer
                    {
                        Id = Guid.Parse("01000000-0000-0000-0000-000000000000"),
                        Name = "David Wilson",
                        Email = "david.wilson@mydomain.com",
                        Phone = "555-321-7890",
                        PermanentAddress = "34 Maple Road, Rivertown, Country",
                        ShippingAddress = "34 Maple Road, Rivertown, Country"
                    }
                });

                context.Products.AddRange(new[]
                {
                    new Product
                    {
                        Id = Guid.Parse("30000000-0000-0000-0000-000000000000"),
                        Name = "Dell Inspiron Laptop",
                        Description = "15.6-inch FHD display, Intel Core i5 processor, 8GB RAM, 512GB SSD, ideal for everyday work and entertainment.",
                        Price = 19.99m
                    },
                    new Product
                    {
                        Id = Guid.Parse("40000000-0000-0000-0000-000000000000"),
                        Name = "Lenovo ThinkPad Laptop",
                        Description = "14-inch FHD display, Intel Core i7 processor, 16GB RAM, 1TB SSD, designed for business performance and durability.",
                        Price = 29.99m
                    },
                    new Product
                    {
                        Id = Guid.Parse("50000000-0000-0000-0000-000000000000"),
                        Name = "HP Pavilion Laptop",
                        Description = "15.6-inch FHD display, AMD Ryzen 5 processor, 16GB RAM, 512GB SSD, suitable for multitasking and daily productivity.",
                        Price = 24.99m
                    },
                    new Product
                    {
                        Id = Guid.Parse("60000000-0000-0000-0000-000000000000"),
                        Name = "Apple MacBook Air",
                        Description = "13.6-inch Retina display, Apple M2 chip, 8GB unified memory, 256GB SSD, lightweight and powerful for professionals.",
                        Price = 49.99m
                    },
                    new Product
                    {
                        Id = Guid.Parse("70000000-0000-0000-0000-000000000000"),
                        Name = "Asus VivoBook Laptop",
                        Description = "14-inch FHD display, Intel Core i5 processor, 16GB RAM, 512GB SSD, compact design for students and office use.",
                        Price = 22.99m
                    }
                });

                context.SaveChanges();
            }
        }
    }
}

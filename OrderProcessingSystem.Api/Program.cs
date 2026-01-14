
using Microsoft.EntityFrameworkCore;
using OrderProcessingSystem.Api.BackgroundServices;
using OrderProcessingSystem.Api.Helper.Extentions;
using OrderProcessingSystem.Api.Helper.Middlewares;
using OrderProcessingSystem.Application.Helper.ServiceRegistries;
using OrderProcessingSystem.Data.Contexts;
using System.Text.Json.Serialization;

namespace OrderProcessingSystem.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddDbContext<OrderProcessingSystemContext>(options =>
            {
                options.UseInMemoryDatabase("OrderProcessingSystemDatabase");
            });

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.DefaultIgnoreCondition =
                        JsonIgnoreCondition.WhenWritingNull;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.RegisterApplicationServices();

            builder.Services.AddHostedService<OrderProcessorBackgroundService>();

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlerMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseAuthorization();

            app.SeedData();

            app.MapControllers();

            app.Run();
        }


    }
}

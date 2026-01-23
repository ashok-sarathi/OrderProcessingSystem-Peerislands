using Microsoft.AspNetCore.Http;
using OrderProcessingSystem.Application.Helper.Exceptions;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Text.Json;

namespace OrderProcessingSystem.Api.Helper.Middlewares
{
    [ExcludeFromCodeCoverage]
    public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var statusCode = ex switch
                {
                    BadRequestException => HttpStatusCode.BadRequest,
                    NotFoundException => HttpStatusCode.NotFound,
                    UnAuthException => HttpStatusCode.Unauthorized,
                    _ => HttpStatusCode.InternalServerError
                };

                var response = new
                {
                    status = (int)statusCode,
                    message = ex.Message,
                    traceId = context.TraceIdentifier
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)statusCode;

                logger.LogError(ex, $"Exception: {ex.Message}");

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }

    public class AuthHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                if(!context.Request.Headers.TryGetValue("X-Auth-Token", out var extractedToken) && extractedToken != "ashok")
                {
                    throw new UnAuthException("Un Auth user");
                }
                await next(context);
            }
            catch
            {
                throw;
            }
        }
    }
}

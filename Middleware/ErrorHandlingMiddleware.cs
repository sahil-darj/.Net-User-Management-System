using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace UserManagementAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // To keep logs, you might log the exception details here
                Console.WriteLine($"Error caught: {ex.Message}");
                await HandleExceptionAsync(context);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var result = JsonSerializer.Serialize(new { error = "Internal server error" });
            return context.Response.WriteAsync(result);
        }
    }
}

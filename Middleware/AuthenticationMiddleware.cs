using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;

namespace UserManagementAPI.Middleware
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public AuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip auth for Swagger
            if (context.Request.Path.Value.StartsWith("/swagger") || 
                context.Request.Path.Value.StartsWith("/favicon.ico"))
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || authHeader != "Bearer mysecrettoken")
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Unauthorized. Invalid token.\"}");
                return;
            }

            await _next(context);
        }
    }
}

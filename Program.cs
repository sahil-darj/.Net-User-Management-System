using UserManagementAPI.Middleware;
using UserManagementAPI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Database configuration (MySQL for XAMPP)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// Add CORS support
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Step 1: Swagger enabled
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User Management API", Version = "v1" });
    
    // Add Security Definition for Testing
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'mysecrettoken' to authenticate."
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            new string[] { }
        }
    });
});

var app = builder.Build();

// Enable CORS
app.UseCors("AllowAll");

// Serve Static Files for Frontend
app.UseDefaultFiles();
app.UseStaticFiles();

// Step 6: Configure middleware in specified order
// 1. ErrorHandlingMiddleware
app.UseMiddleware<ErrorHandlingMiddleware>();

// 2. AuthenticationMiddleware
app.UseMiddleware<AuthenticationMiddleware>();

// 3. LoggingMiddleware
app.UseMiddleware<LoggingMiddleware>();

// Enable Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// MapControllers must be called
app.MapControllers();

app.Run();

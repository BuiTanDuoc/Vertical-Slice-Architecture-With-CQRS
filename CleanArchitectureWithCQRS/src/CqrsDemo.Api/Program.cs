using System.Reflection;
using CqrsDemo.Api.Middleware;
using CqrsDemo.Application;
using CqrsDemo.Application.Products.Commands.CreateProduct;
using CqrsDemo.Infrastructure;
using CqrsDemo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CqrsDemo API",
        Version = "v1",
        Description = "Demo API dùng Clean Architecture + CQRS (không dùng MediatR)."
    });

    // Api project's own XML comments (controller summaries, [ProducesResponseType] etc.)
    var apiXml = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
    if (File.Exists(apiXml))
        c.IncludeXmlComments(apiXml);

    // Application project's XML comments (Command/Query/DTO property descriptions) -
    // these types are the actual request/response schemas Swagger renders.
    var applicationXml = Path.Combine(AppContext.BaseDirectory, $"{typeof(CreateProductCommand).Assembly.GetName().Name}.xml");
    if (File.Exists(applicationXml))
        c.IncludeXmlComments(applicationXml);
});

// Each layer exposes its own AddXxx() extension - Program.cs just wires them together.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CqrsDemo API v1");
    });

    // Convenience for local testing only - applies any pending migrations on startup.
    // Remove/guard this before deploying anywhere real; migrations should run as a
    // deliberate deploy step, not implicitly on app boot.
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

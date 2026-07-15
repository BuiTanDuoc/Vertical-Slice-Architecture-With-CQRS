using Carter;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VerticalSliceArchitectureWithCQRS.Data;
using VerticalSliceArchitectureWithCQRS.Share.Behaviors;
using VerticalSliceArchitectureWithCQRS.Share.CQRS;
using VerticalSliceArchitectureWithCQRS.Share.Exceptions.Handler;

var builder = WebApplication.CreateBuilder(args);

var assembly = typeof(Program).Assembly;
//instead of above, adding Scrutor 
builder.Services.Scan(scan => scan.FromAssembliesOf(typeof(Program))
    .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)), publicOnly: false)
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
    .AsImplementedInterfaces()
    .WithScopedLifetime()
    .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)), publicOnly: false)
    .AsImplementedInterfaces()
    .WithScopedLifetime());
builder.Services.AddScoped<IDispatcher, Dispatcher>();

builder.Services.AddValidatorsFromAssembly(assembly, includeInternalTypes: true);

builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddCarter();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Vertical Slice Architecture API", Version = "v1" });
});
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddConsole();
    loggingBuilder.AddDebug();
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CustomerContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CustomerContext>();
    db.Database.Migrate();
}

app.MapOpenApi();
app.MapScalarApiReference();
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vertical Slice Architecture API V1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseExceptionHandler(options => { });

app.MapCarter();

app.Run();

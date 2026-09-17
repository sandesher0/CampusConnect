using API.ExceptionHandlers;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Modules.Auth.Facades;
using Modules.Auth.Ports;
using SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Add MVC services for controllers
builder.Services.AddControllers();

// Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Dependency Injection
builder.Services.AddScoped<
    IUserRegistrationRepository,
    UserRegistrationRepository>();

builder.Services.AddScoped<
    IUserRegistrationFacade,
    UserRegistrationFacade>();


builder.Services.AddSingleton<
    IPasswordHasher,
    PasswordHasher>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Global Exception Handler
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
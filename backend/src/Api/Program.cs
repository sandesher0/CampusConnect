using System.Text;
using API.ExceptionHandlers;
using Application.Ports;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Modules.Auth.Facades;
using Modules.Auth.Options;
using Modules.Auth.Ports;
using SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

// Add MVC services for controllers
builder.Services.AddControllers();

builder.Services.Configure<JwtOptions>(
       builder.Configuration.GetSection(JwtOptions.SectionName));

// Exception Handling
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// Dependency Injection
builder.Services.AddScoped<
    Modules.Auth.Ports.IAccountRepository,
    AccountRepository>();

builder.Services.AddScoped<
    Modules.Users.Ports.IUserRegistrationRepository,
    UserRegistrationRepository>();

builder.Services.AddScoped<
    Modules.Auth.Ports.IAccountRegistrationFacade,
    AccountRegistrationFacade>();

builder.Services.AddScoped<
    Modules.Users.Ports.IUserRegistrationFacade,
    Modules.Users.Facades.UserRegistrationFacade>();

builder.Services.AddScoped<
    IAccountLoginFacade,
    LoginFacade>();


builder.Services.AddSingleton<
    IPasswordHasher,
    PasswordHasher>();

builder.Services.AddSingleton<
    ITokenGenerator,
    TokenGenerator>();

builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]!)),
        ClockSkew = TimeSpan.Zero,
    };
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
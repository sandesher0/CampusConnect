using System.Text;
using API.ExceptionHandlers;
using Application.Facades;
using Application.Ports;
using Application.UseCase;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Modules.Auth.Facades;
using Modules.Auth.Services;
using Modules.Auth.Options;
using Modules.Auth.Ports;
using Modules.Users.Facades;
using Modules.Users.Ports;
using SharedKernel.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

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
    IUserRegistrationRepository,
    UserRegistrationRepository>();

builder.Services.AddScoped<
    IAccountRegistrationFacade,
    AccountRegistrationFacade>();

builder.Services.AddScoped<
    IUserRegistrationFacade,
    UserRegistrationFacade>();

builder.Services.AddScoped<
    IAccountLoginFacade,
    LoginFacade>();

builder.Services.AddScoped<
    IGetCurrentUserFacade,
    GetCurrentUserFacade>();


builder.Services.AddSingleton<
    IPasswordHasher,
    PasswordHasher>();

builder.Services.AddSingleton<
    ITokenGenerator,
    TokenGenerator>();

builder.Services.AddScoped<
    IChangePasswordFacade,
    ChangePasswordFacade>();

builder.Services.AddScoped<IRegisterUserUseCase, RegisterUserUseCase>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork<AppDbContext>>();
builder.Services.AddHttpContextAccessor();



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
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["accessToken"];
            return Task.CompletedTask;
        }
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

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Options;
using Packbuilder.Services;
using dotenv.net;
using Microsoft.IdentityModel.Tokens;
using System.Text;

[assembly: InternalsVisibleTo("Packbuilder.Tests")]

var builder = WebApplication.CreateBuilder(args);

#region Services
DotEnv.Load();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// builder.Services.AddIdentityCore<User>();
builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IVersionService, VersionService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddLogging((options) =>
{
    options.AddConsole();
    options.AddDebug();
    options.SetMinimumLevel(LogLevel.Debug);
});
builder.Services.AddHttpLogging();
builder.Services.AddSingleton<JwtOptions>(pp =>
{
    return builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
        ?? throw new InvalidOperationException("Jwt options are not configured properly.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "https://api.packbuilder.io",
        ValidAudience = "https://packbuilder.io",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
    };
});
builder.Services.AddDbContext<PackbuilderContext>(opt =>
{
    DbOptions options = builder.Configuration.GetSection("DB").Get<DbOptions>()!;
    opt.UseNpgsql(options.ConnectionString);
});
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

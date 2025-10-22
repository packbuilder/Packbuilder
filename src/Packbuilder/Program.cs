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
using Packbuilder.Data;
using Azure.Core;

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
    PasswordHasher<User> passwordHasher = new();
    opt.UseNpgsql(options.ConnectionString);
    opt.UseAsyncSeeding(async (context, _, _) =>
    {
        SeedService seedService = new((context as PackbuilderContext)!, passwordHasher);
        if (builder.Environment.IsDevelopment() && !await context.Set<User>().AnyAsync())
        {
            await seedService.Seed();
        }
    });

    opt.UseSeeding((context, _) =>
    {
        SeedService seedService = new((context as PackbuilderContext)!, passwordHasher);
        if (builder.Environment.IsDevelopment() && !context.Set<User>().Any())
        {
            seedService.Seed().GetAwaiter().GetResult();
        }
    });
});
builder.Services.AddHttpClient<ICurseForgeApiService, CurseForgeApiService>(
    client =>
    {
        string? baseUrl = builder.Configuration["CurseForgeApi:BaseUrl"];
        string? apiKey = Environment.GetEnvironmentVariable("CURSEFORGE_API_KEY");

        if (baseUrl is null)
        {
            throw new InvalidOperationException("CurseForgeApi:BaseUrl configuration not found in appsettings.json");
        }
        
        if(apiKey is null)
        {
            throw new InvalidOperationException("Curseforge api key not set in environment variable");
        }

        client.BaseAddress = new Uri(baseUrl);
        client.DefaultRequestHeaders.Add("x-api-key", apiKey);
        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }
);
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

app.UseCors(opt =>
{
    opt.WithOrigins("http://localhost:5173")
       .AllowAnyMethod()
       .AllowAnyHeader()
       .AllowCredentials();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

using Packbuilder.Interfaces;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Models;
using Packbuilder.Options;
using Packbuilder.Services;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Packbuilder.Data;
using CurseForge.Interfaces;
using CurseForge.Services;
using CurseForge.Options;
using StackExchange.Redis;
using Packbuilder.Jobs;
using Packbuilder.Events.Hubs;
using Packbuilder.Events;
using Packbuilder.Events.Handlers;
using Microsoft.AspNetCore.Authorization;
using Packbuilder.Policys;
using Packbuilder.Config;
using Packbuilder.Middleware;

[assembly: InternalsVisibleTo("Packbuilder.Tests")]

var builder = WebApplication.CreateBuilder(args);
var curseForgeApiKey = builder.Configuration["Curseforge:ApiKey"];

#region Services
builder.Configuration.AddEnvironmentVariables();
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddSwaggerGen();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
// builder.Services.AddIdentityCore<User>();
builder.Services.AddTransient<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddTransient<ISessionService, SessionService>();
builder.Services.AddTransient<IVersionService, VersionService>();
builder.Services.AddTransient<IVersionModService, VersionModService>();
builder.Services.AddTransient<ISuggestionService, SuggestionService>();
builder.Services.AddTransient<IModService, ModService>();
builder.Services.AddTransient<IModpackService, ModpackService>();
builder.Services.AddTransient<IEventDispatcher, EventDispatcher>();
builder.Services.AddTransient<IEventHandler<SuggestionUpdatedEvent>, SuggestionUpdatedHandler>();
builder.Services.AddTransient<IEventHandler<ModpackUpdatedEvent>, ModpackUpdatedHandler>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IModificationService, ModificationService>();
builder.Services.AddTransient<ICacheService, CacheService>();
builder.Services.AddTransient<IBookmarkService, BookmarkService>();
builder.Services.AddTransient<ITokenService, TokenService>();
builder.Services.AddHostedService<VerificationJob>();
builder.Services.AddHostedService<MergeSuggestionJob>();
builder.Services.AddHostedService<SendVerificationEmailJob>();
builder.Services.AddHostedService<SendPasswordResetEmailJob>();
builder.Services.AddScoped<ICurseForgeCacheService, CurseForgeCacheService>();
builder.Services.AddScoped<ICurseForgeManifestService, CurseForgeManifestService>();
builder.Services.AddScoped<ICurseForgeService, CurseForgeService>();
builder.Services.AddScoped<ICurseForgeApiService, CurseForgeApiService>();
builder.Services.AddScoped<IAuthorizationHandler, EmailVerifiedHandler>();
builder.Services.AddScoped<RateLimitMiddleware>();
builder.Services.AddHttpClient<IEmailService, BrevoEmailService>();
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
        ValidIssuer = "https://api.packbuilder.org",
        ValidAudience = "https://packbuilder.org",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key))
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            string? token = context.Request.Cookies["access_token"];

            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }

            // THIS is required for SignalR
            string? accessToken = context.Request.Query["access_token"];

            PathString path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("VerifiedEmail", policy =>
        policy.RequireAuthenticatedUser()
              .AddRequirements(new EmailVerifiedRequirement()));

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

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:3000",
                "http://localhost:5173",
                "https://packbuilder.org")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.Configure<BrevoSettings>(builder.Configuration.GetSection("Brevo"));
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddSingleton(new CurseForgeApiOptions {
    ApiKey = curseForgeApiKey!,
    BaseUrl = builder.Configuration["Curseforge:BaseUrl"] ?? throw new Exception("Must set curseforge api url")
});

builder.Services.AddSingleton<IConnectionMultiplexer>((_) => {
        return ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")!);
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

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RateLimitMiddleware>();
app.MapControllers();

app.MapHub<ModpackHub>("/hubs/modpacks");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
    context.Database.Migrate();
}

app.Run();
using System.Net.Http.Json;
using Bogus;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.Create;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Options;
using Packbuilder.Services;
using Packbuilder.Tests.Factories;
using StackExchange.Redis;

namespace Packbuilder.Tests
{
    public class ApplicationTests
    {
        protected static IServiceProvider Services { get; set; } = null!;
        protected static PackbuilderContext Context { get; set; } = null!;
        protected static string TestConnectionString { get; private set; } = null!;

        [ClassInitialize]
        public static void BaseSetup(TestContext _)
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddUserSecrets<ApplicationTests>()
                .Build();

            DbOptions options = configuration.GetSection("Db").Get<DbOptions>() 
                ?? throw new InvalidOperationException("Db configuration could not be loaded. Verify your user-secrets are set up.");
            
            JwtOptions jwtOptions = configuration.GetSection("Jwt").Get<JwtOptions>() ?? throw new Exception("Jwt configuration could not be loaded. Verify your user-secrets are set up.");

            string redisConnectionString = configuration.GetConnectionString("Redis") ?? throw new Exception("Redis connection string could not be found. Verify your user-secrets are set up.");

            if (string.IsNullOrWhiteSpace(options.TestConnectionString))
            {
                throw new InvalidOperationException("Database connection string is empty or not set.");
            }

            if (string.IsNullOrWhiteSpace(redisConnectionString))
            {
                throw new InvalidOperationException("Redis connection string is empty or not set.");
            }

            TestConnectionString = options.TestConnectionString;

            Services = new ServiceCollection()
                .AddDbContext<PackbuilderContext>(opt => opt.UseNpgsql(TestConnectionString))
                .AddSingleton<JwtOptions>(pp => jwtOptions)
                .AddSingleton<IConnectionMultiplexer>((_) => ConnectionMultiplexer.Connect(redisConnectionString))
                .AddTransient<IPasswordHasher<User>, PasswordHasher<User>>()
                .AddTransient<ISessionService, SessionService>()
                .AddTransient<IVersionService, VersionService>()
                .AddTransient<ISuggestionService, SuggestionService>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<ModpackFactory>()
                .AddSingleton<ModFactory>()
                .AddSingleton<NameFactory>()
                .AddSingleton<UserFactory>()
                .AddSingleton<SuggestionFactory>()
                .AddSingleton<Faker>()
                .BuildServiceProvider();

            Context = Services.GetRequiredService<PackbuilderContext>();
            Context.Database.EnsureDeleted();
            Context.Database.Migrate();
        }
        
        public static async Task<HttpClient> CreateSessionClient(User user, string password)
        {
            PackbuilderWebApplicationFactory application = new(TestConnectionString);

            HttpClient client = application.CreateClient(new WebApplicationFactoryClientOptions()
            {
                HandleCookies = true
            });

            await client.PostAsJsonAsync("/sessions/login", new CreateSessionDto()
            {
                Email = user.Email,
                Password = password
            });

            return client;
        }
    }    
}
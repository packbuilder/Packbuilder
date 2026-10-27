using System.Net.Http.Headers;
using System.Net.Http.Json;
using Bogus;
using dotenv.net;
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
            string envPath = Path.Combine(AppContext.BaseDirectory, ".env.test");

            if (!File.Exists(envPath))
            {
                throw new FileNotFoundException("Test environment file was not found.", envPath);
            }

            // .env.test file must contain db connection string alongside curseforge api key
            DotEnv.Load(new DotEnvOptions(envFilePaths: [envPath], overwriteExistingVars: false, ignoreExceptions: false));

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.testing.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            DbOptions options = configuration.GetSection("Db").Get<DbOptions>() 
                ?? throw new InvalidOperationException("Db configuration could not be loaded. Check your environment variables.");

            if (string.IsNullOrWhiteSpace(options.ConnectionString))
            {
                throw new InvalidOperationException("Database connection string is missing.");
            }

            TestConnectionString = options.ConnectionString;

            Services = new ServiceCollection()
                .AddDbContext<PackbuilderContext>(opt =>
                {
                    opt.UseNpgsql(TestConnectionString);
                }).AddSingleton<JwtOptions>(pp =>
                {
                    return configuration.GetSection("Jwt").Get<JwtOptions>()!;
                })
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
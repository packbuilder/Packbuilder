using System.Net.Http.Headers;
using Bogus;
using dotenv.net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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

        [ClassInitialize]
        public static void BaseSetup(TestContext _)
        {
            DotEnv.Load();

            IConfiguration configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();

            Services = new ServiceCollection()
                .AddDbContext<PackbuilderContext>(opt =>
                {
                    DbOptions options = configuration.GetSection("Db").Get<DbOptions>()!;
                    opt.UseNpgsql(options.ConnectionString);
                })
                .AddSingleton<JwtOptions>(pp =>
                {
                    return configuration.GetSection("Jwt").Get<JwtOptions>()!;
                })
                .AddTransient<IPasswordHasher<User>, PasswordHasher<User>>()
                .AddTransient<ISessionService, SessionService>()
                .AddTransient<IVersionService, VersionService>()
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
            Context.Database.EnsureCreated();
        }
        
        public static async Task<HttpClient> CreateSessionClient(User user, string password)
        {
            PackbuilderWebApplicationFactory application = new();
            ISessionService sessions = application.Services.GetRequiredService<ISessionService>();
            string token = await sessions.CreateSession(new CreateSessionDto()
            {
                Email = user.Email,
                Password = password
            });
            HttpClient client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }
    }    
}
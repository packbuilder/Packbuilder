using Bogus;
using dotenv.net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
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
                .AddSingleton<UserFactory>()
                .AddSingleton<JwtOptions>(pp =>
                {
                    return configuration.GetSection("Jwt").Get<JwtOptions>()!;
                })
                .AddTransient<IPasswordHasher<User>, PasswordHasher<User>>()
                .AddTransient<ISessionService, SessionService>()
                .AddSingleton<IHttpContextAccessor, HttpContextAccessor>()
                .AddSingleton<NameFactory>()
                .AddSingleton<Faker>()
                .BuildServiceProvider();
                
            Context = Services.GetRequiredService<PackbuilderContext>();
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }
    }    
}
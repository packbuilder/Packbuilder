using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Models;
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
            IServiceCollection Builder = new ServiceCollection()
                .AddDbContext<PackbuilderContext>(opt =>
                {
                    //Figure out how to set up database and run tests
                    opt.UseNpgsql();
                })
                .AddSingleton<UserFactory>()
                .AddSingleton<NameFactory>()
                .AddSingleton<Faker>();

            Builder.AddIdentityCore<User>();
            Services = Builder.BuildServiceProvider();

            Context = Services.GetRequiredService<PackbuilderContext>();
            Context.Database.EnsureDeleted();
            Context.Database.EnsureCreated();
        }
    }    
}
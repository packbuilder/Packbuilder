using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bogus;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.Create;
using Packbuilder.Models;
using Packbuilder.Tests.Factories;

namespace Packbuilder.Tests.Api;

[TestClass]
public class ModpackTest : ApplicationTests
{
    [ClassInitialize]
    public static void Setup(TestContext ctx)
    {
        BaseSetup(ctx);
    }

    [TestMethod]
    public async Task GetModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"{user.Name}/modpacks/{modpack.Slug}");
        
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    public async Task CreateModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();
        var modFactory = Services.GetRequiredService<ModFactory>();

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);

        List<Mod> mods = [];
        float latest = 0.0f;

        for (int i = 0; i < 3; i++)
        {
            Mod newMod = modFactory.CreateMod();
            mods.Add(newMod);
            modpack.CreateVersion(mods);
            latest += .1f;
        }

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        Faker faker = new();
        HttpClient client = await CreateSessionClient(user, password);
        PackbuilderWebApplicationFactory application = new();
        JsonContent data = JsonContent.Create(new CreateModpackDto
        {
            Name = faker.Name.FirstName(),
        });
        HttpResponseMessage res = await client.PostAsync($"{user.Name}/modpacks/CreateModpack", data);
        
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
}

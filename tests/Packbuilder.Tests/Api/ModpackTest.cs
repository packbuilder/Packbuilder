using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
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

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"{user.Name}/modpacks/{modpack.Slug}");
        // TODO: Figure out how to read the response in its object format from httpresponsemessage in order to do proper comparing for your tests
        string? resModpack = await res.Content.ReadAsStringAsync();


        Console.WriteLine(resModpack);

        // TODO: Double check my data models and make sure that there isnt a loop due to things referencing themselves
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.IsNotNull(resModpack);
        // Assert.AreEqual(latest, resModpack.Versions.First().Iteration);
        // Assert.AreEqual(modpack.Id, resModpack.Id);
    }
}

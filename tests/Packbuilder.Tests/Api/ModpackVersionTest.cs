using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Models;
using Packbuilder.Tests;
using Packbuilder.Tests.Factories;

[TestClass]
public class ModpackVersionTest : ApplicationTests
{

    [ClassInitialize]
    public static void Setup(TestContext ctx)
    {
        BaseSetup(ctx);
    }

    [TestMethod]
    public async Task GetVersionModTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var modFactory = Services.GetRequiredService<ModFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);
        List<Mod> mods = [];

        for (int i = 0; i < 3; i++)
        {
            Mod newMod = modFactory.CreateMod();
            mods.Add(newMod);
            ModpackVersion version = modpack.CreateVersion(mods);
            context.Versions.Add(version);
            context.Mods.Add(newMod);
        }

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        ModpackVersion latestVersion = modpack.Versions.Last();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"{user.Name}/modpacks/{modpack.Slug}/versions/{latestVersion.Id}");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    // [TestMethod]
    // public async Task CreateVersionModTest()
    // {
        
    // }
}
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Services;
using Packbuilder.Tests;
using Packbuilder.Tests.Factories;

namespace Packbuilder.Tests.Api;

[TestClass]
public class VersionTest() : ApplicationTests
{

    [ClassInitialize]
    public static void Setup(TestContext ctx)
    {
        BaseSetup(ctx);
    }

    [TestMethod]
    public async Task GetVersionTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var versionService = Services.GetRequiredService<IVersionService>();
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
            ModpackVersion version = modpack.CreateVersion(mods, modpack);
            context.Versions.Add(version);
            context.Mods.Add(newMod);
        }

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        VersionDto latestVersion = await versionService.GetLatestVersion(modpack.Id) ?? throw new Exception("No latest version found.");

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"{user.Name}/modpacks/{modpack.Slug}/versions/{latestVersion.Id}");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    [TestMethod]
    public async Task CreateVersionTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var modFactory = Services.GetRequiredService<ModFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);

        for (int i = 0; i < 3; i++)
        {
            Mod newMod = modFactory.CreateMod();
            context.Mods.Add(newMod);
        }

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        List<int> modIds = await context.Mods.Select(m => m.Id).ToListAsync();

        HttpClient client = await CreateSessionClient(user, password);
        JsonContent data = JsonContent.Create(new CreateVersionDto
        {
            ModIds = modIds
        });
        HttpResponseMessage res = await client.PostAsync($"{user.Name}/modpacks/{modpack.Slug}/versions", data);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
}
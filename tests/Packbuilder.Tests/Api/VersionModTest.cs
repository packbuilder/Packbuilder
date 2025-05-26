using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;
using Packbuilder.Tests.Factories;

namespace Packbuilder.Tests.Api;

[TestClass]
public class VersionModTest() : ApplicationTests
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
            ModpackVersion version = modpack.CreateVersionFromMods(mods, "1.20", ModLoader.Forge);

            foreach (VersionMod versionMod in version.VersionMods)
            {
                context.VersionMods.Add(versionMod);
            }

            context.Versions.Add(version);
            context.Mods.Add(newMod);
        }


        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        VersionDto latestVersion = await versionService.GetLatestVersion(modpack.Id) ?? throw new Exception("No latest version found.");

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"{user.Name}/modpacks/{modpack.Slug}/versions/{latestVersion.Id}/mods");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
}
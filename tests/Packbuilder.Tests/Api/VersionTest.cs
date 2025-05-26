using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;
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
            ModpackVersion version = modpack.CreateVersionFromMods(mods, "1.20", ModLoader.Forge);
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
        var suggestionFactory = Services.GetRequiredService<SuggestionFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);
        Mod mod = modFactory.CreateMod();
        List<Mod> mods = [mod];
        ModpackVersion version = modpack.CreateVersionFromMods(mods, "1.20", ModLoader.Forge);
        Suggestion suggestion = suggestionFactory.CreateSuggestion(user, modpack, version);

        for (int i = 0; i < 3; i++)
        {
            Mod newMod = modFactory.CreateMod();
            context.Mods.Add(newMod);
        }

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Mods.Add(mod);
        context.Versions.Add(version);
        context.Modpacks.Add(modpack);
        context.Suggestions.Add(suggestion);
        await context.SaveChangesAsync();

        List<Mod> dbMods = await context.Mods.Where(m => m.Platform == ModPlatform.Thunderstore).ToListAsync();
        Suggestion? dbSuggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.ModpackId == modpack.Id) ?? throw new Exception("Suggestion doesnt exist");

        foreach (Mod item in dbMods)
        {
            Modification modification = new()
            {
                ModId = item.Id,
                ModAction = ModAction.Added,
                SuggestionId = suggestion.Id,
                ConflictState = ConflictState.NoConflicts
            };

            dbSuggestion.Modifications.Add(modification);
            context.Modifications.Add(modification);
        }

        context.Suggestions.Update(dbSuggestion);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.PostAsync($"{user.Name}/modpacks/{modpack.Slug}/versions/{suggestion.Id}", new StringContent(string.Empty));

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
}
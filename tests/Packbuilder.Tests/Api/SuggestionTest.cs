using System.Net;
using System.Net.Http.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Models;
using Packbuilder.Models.enums;
using Packbuilder.Tests.Factories;

namespace Packbuilder.Tests.Api;

[TestClass]
public class SuggestionTest : ApplicationTests
{
    [ClassInitialize]
    public static void Setup(TestContext ctx)
    {
        BaseSetup(ctx);
    }

    [TestMethod]
    public async Task GetSuggestionsTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var modFactory = Services.GetRequiredService<ModFactory>();
        var suggestionFactory = Services.GetRequiredService<SuggestionFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user);
        Mod mod = modFactory.CreateMod();
        List<Mod> mods = [mod];
        ModpackVersion modpackVersion = modpack.CreateVersionFromMods(mods, "1.20", ModLoader.Forge);
        Suggestion suggestion = suggestionFactory.CreateSuggestion(user, modpack, modpackVersion);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Versions.Add(modpackVersion);
        context.Mods.Add(mod);
        context.Modpacks.Add(modpack);
        context.Suggestions.Add(suggestion);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"{user.Name}/modpacks/{modpack.Slug}/suggestions");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
    [TestMethod]
    public async Task CreateSuggestionsTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var userFactory = Services.GetRequiredService<UserFactory>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var suggestionFactory = Services.GetRequiredService<SuggestionFactory>();
        var modFactory = Services.GetRequiredService<ModFactory>();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user);
        Mod mod = modFactory.CreateMod();
        List<Mod> mods = [mod];

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);
        ModpackVersion version = modpack.CreateVersionFromMods(mods, "1.20", ModLoader.Forge);
        Suggestion suggestion = suggestionFactory.CreateSuggestion(user, modpack, version);

        context.Users.Add(user);
        context.Mods.Add(mod);
        context.Modpacks.Add(modpack);
        context.Versions.Add(version);
        context.Suggestions.Add(suggestion);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        JsonContent data = JsonContent.Create(new CreateSuggestionDto
        {
            Memo = suggestion.Memo,
            GameVersion = "1.20",
            ModLoader = ModLoader.Forge
        });
        HttpResponseMessage res = await client.PostAsync($"{user.Name}/modpacks/{modpack.Slug}/suggestions", data);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
    [TestMethod]
    public async Task UpdateSuggestionsTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();
        var modFactory = Services.GetRequiredService<ModFactory>();
        var suggestionFactory = Services.GetRequiredService<SuggestionFactory>();
        Faker faker = new();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user);
        Mod mod = modFactory.CreateMod();
        List<Mod> mods = [mod];
        ModpackVersion version = modpack.CreateVersionFromMods(mods, "1.20", ModLoader.Forge);
        Suggestion suggestion = suggestionFactory.CreateSuggestion(user, modpack, version);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        context.Mods.Add(mod);
        context.Versions.Add(version);
        context.Suggestions.Add(suggestion);
        await context.SaveChangesAsync();

        CreateSuggestionDto updateSuggestionDto = new()
        {
            Memo = faker.Lorem.Paragraph(),
            GameVersion = "1.20",
            ModLoader = ModLoader.Forge
        };

        JsonContent data = JsonContent.Create(updateSuggestionDto);

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.PutAsync($"{user.Name}/modpacks/{modpack.Slug}/suggestions/{suggestion.Id}", data);

        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        Suggestion? updatedSuggestion = await secondContext.Suggestions.SingleOrDefaultAsync(m => m.Id == modpack.Id);

        Assert.IsNotNull(updatedSuggestion);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.AreNotEqual(suggestion.Memo, updatedSuggestion.Memo);
    }
    [TestMethod]
    public async Task DeleteSuggestionsTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var modFactory = Services.GetRequiredService<ModFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();
        var suggestionFactory = Services.GetRequiredService<SuggestionFactory>();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user);
        Mod mod = modFactory.CreateMod();
        List<Mod> mods = [mod];
        ModpackVersion version = modpack.CreateVersionFromMods(mods, "1.20", ModLoader.Forge);
        Suggestion suggestion = suggestionFactory.CreateSuggestion(user, modpack, version);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        context.Mods.Add(mod);
        context.Versions.Add(version);
        context.Suggestions.Add(suggestion);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.DeleteAsync($"{user.Name}/modpacks/{modpack.Slug}/suggestions/{suggestion.Id}");

        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        Suggestion? deletedSuggestion = await secondContext.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestion.Id);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.IsNull(deletedSuggestion);
    }    
}
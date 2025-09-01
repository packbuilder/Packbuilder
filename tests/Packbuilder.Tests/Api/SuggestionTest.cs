using System.Net;
using System.Net.Http.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Models;
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
        var suggestionFactory = Services.GetRequiredService<SuggestionFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);
        Suggestion suggestion = suggestionFactory.CreateSuggestion(user, modpack);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
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

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);
        Suggestion suggestion = suggestionFactory.CreateSuggestion(user, modpack);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        JsonContent data = JsonContent.Create(new CreateSuggestionDto
        {
            Memo = suggestion.Memo,
        });
        HttpResponseMessage res = await client.PostAsync($"{user.Name}/modpacks/{modpack.Slug}/suggestions", data);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
    // [TestMethod]
    // public async Task UpdateSuggestionsTest()
    // {
    //     var context = Services.GetRequiredService<PackbuilderContext>();
    //     var modpackFactory = Services.GetRequiredService<ModpackFactory>();
    //     var userFactory = Services.GetRequiredService<UserFactory>();
    //     Faker faker = new();

    //     (User user, string password) = userFactory.CreateUser();
    //     Modpack modpack = modpackFactory.CreateModpack(user);

    //     modpack.Slug = Modpack.GenerateSlug(modpack.Name);

    //     context.Users.Add(user);
    //     context.Modpacks.Add(modpack);
    //     await context.SaveChangesAsync();

    //     UpdateModpackDto updateModpackDto = new UpdateModpackDto
    //     {
    //         Name = faker.Name.FirstName(),
    //         Avatar = faker.Internet.Avatar(),
    //     };

    //     JsonContent data = JsonContent.Create(updateModpackDto);

    //     HttpClient client = await CreateSessionClient(user, password);
    //     HttpResponseMessage res = await client.PutAsync($"{user.Name}/modpacks/{modpack.Slug}", data);

    //     var scope = Services.CreateScope();
    //     var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
    //     Modpack? updatedModpack = await secondContext.Modpacks.SingleOrDefaultAsync(m => m.Id == modpack.Id);

    //     Assert.IsNotNull(updatedModpack);
    //     Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    //     Assert.AreNotEqual(modpack.Name, updatedModpack.Name);
    //     Assert.AreNotEqual(modpack.Avatar, updatedModpack.Avatar);
    // }
    // [TestMethod]
    // public async Task DeleteSuggestionsTest()
    // {
    //     var context = Services.GetRequiredService<PackbuilderContext>();
    //     var modpackFactory = Services.GetRequiredService<ModpackFactory>();
    //     var userFactory = Services.GetRequiredService<UserFactory>();

    //     (User user, string password) = userFactory.CreateUser();
    //     Modpack modpack = modpackFactory.CreateModpack(user);

    //     modpack.Slug = Modpack.GenerateSlug(modpack.Name);

    //     context.Users.Add(user);
    //     context.Modpacks.Add(modpack);
    //     await context.SaveChangesAsync();

    //     HttpClient client = await CreateSessionClient(user, password);
    //     HttpResponseMessage res = await client.DeleteAsync($"{user.Name}/modpacks/{modpack.Slug}");


    //     var scope = Services.CreateScope();
    //     var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
    //     Modpack? deletedModpack = await secondContext.Modpacks.SingleOrDefaultAsync(m => m.Id == modpack.Id);

    //     Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    //     Assert.IsNull(deletedModpack);
    // }    
}
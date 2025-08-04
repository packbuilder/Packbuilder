using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Models;
using Packbuilder.Tests.Factories;

namespace Packbuilder.Tests.Api;

//TODO: Try to create and update the new modpack with it's new versions. to do this, consider how the data will be stored within the data base and how your dtos that you created reflect that data transfer/storage. 

//TODO: Handle and read data using the new dtos you created in order to check if versions and iterations are working within your project

//TODO: Use asserts to check

// List<Mod> mods = [];
// float latest = 0.0f;

// for (int i = 0; i < 3; i++)
// {
//     Mod newMod = modFactory.CreateMod();
//     mods.Add(newMod);
//     modpack.CreateVersion(mods);
//     latest += .1f;
// }

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

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"{user.Name}/modpacks/{modpack.Slug}");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
    [TestMethod]
    public async Task CreateModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser();

        context.Users.Add(user);
        await context.SaveChangesAsync();

        Faker faker = new();
        HttpClient client = await CreateSessionClient(user, password);
        JsonContent data = JsonContent.Create(new CreateModpackDto
        {
            Name = faker.Name.FirstName(),
        });
        HttpResponseMessage res = await client.PostAsync($"{user.Name}/modpacks", data);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
    [TestMethod]
    public async Task UpdateModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();
        Faker faker = new();

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        UpdateModpackDto updateModpackDto = new UpdateModpackDto
        {
            Name = faker.Name.FirstName(),
            Avatar = faker.Internet.Avatar(),
        };

        JsonContent data = JsonContent.Create(updateModpackDto);

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.PutAsync($"{user.Name}/modpacks/{modpack.Slug}", data);

        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        Modpack? updatedModpack = await secondContext.Modpacks.SingleOrDefaultAsync(m => m.Id == modpack.Id);

        Assert.IsNotNull(updatedModpack);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.AreNotEqual(modpack.Name, updatedModpack.Name);
        Assert.AreNotEqual(modpack.Avatar, updatedModpack.Avatar);
    }
    [TestMethod]
    public async Task DeleteModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser();
        Modpack modpack = modpackFactory.CreateModpack(user);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.DeleteAsync($"{user.Name}/modpacks/{modpack.Slug}");


        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        Modpack? deletedModpack = await secondContext.Modpacks.SingleOrDefaultAsync(m => m.Id == modpack.Id);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.IsNull(deletedModpack);
    }    
}

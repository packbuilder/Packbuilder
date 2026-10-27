using System.IO.Compression;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Bogus;
using CurseForge.Dtos.CurseForgeApiDtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NuGet.ContentModel;
using Packbuilder.Dto;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Models;
using Packbuilder.Models.enums;
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
    public async Task GetUserModpacksTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user, Game.Minecraft);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"/modpacks/user-modpacks/{user.Id}");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    [TestMethod]
    public async Task GetModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user, Game.Minecraft);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"/modpacks/{modpack.Id}");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    [TestMethod]
    public async Task CreateModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser(true);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        Faker faker = new();
        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.PostAsJsonAsync($"/modpacks", new CreateModpackDto
        {
            Name = faker.Name.FirstName(),
            GameVersion = "1.20",
            ModLoader = ModLoader.Forge,
            ImageType = ImageType.Stock,
            ImageValue = "avatar_1.jpg",
            Game = Game.Minecraft
        });

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }
    [TestMethod]
    public async Task UpdateModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();
        Faker faker = new();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user, Game.Minecraft);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        UpdateModpackDto updateModpackDto = new()
        {
            Name = faker.Name.FirstName(),
            ImageType = ImageType.Stock,
            ImageValue = "modpack_avatar_2.gif"
        };

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.PutAsJsonAsync($"/modpacks/{modpack.Id}", updateModpackDto);

        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        Modpack? updatedModpack = await secondContext.Modpacks.SingleOrDefaultAsync(m => m.Id == modpack.Id);

        Assert.IsNotNull(updatedModpack);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.AreNotEqual(modpack.Name, updatedModpack.Name);
        Assert.AreNotEqual(modpack.ImageValue, updatedModpack.ImageValue);
    }
    [TestMethod]
    public async Task DeleteModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user, Game.Minecraft);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.DeleteAsync($"/modpacks");


        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        Modpack? deletedModpack = await secondContext.Modpacks.SingleOrDefaultAsync(m => m.Id == modpack.Id);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.IsNull(deletedModpack);
    }

    // TODO: Create test for importing modpack

    [TestMethod]
    public async Task ImportMinecraftModpackTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var userFactory = Services.GetRequiredService<UserFactory>();

        (User user, string password) = userFactory.CreateUser(true);

        context.Users.Add(user);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);

        string manifestPath = Path.Combine(AppContext.BaseDirectory, "TestFiles", "manifest.json");
        
        await using FileStream fileStream = File.OpenRead(manifestPath);
        using MultipartFormDataContent multipartContent = new();
        using StreamContent streamContent = new(fileStream);

        streamContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
        
        AvatarDto avatarDto = new()
        {
            ImageType = ImageType.Stock,
            ImageValue = "modpack_avatar_1.gif"
        };
        string avatarJson = JsonSerializer.Serialize(avatarDto);

        multipartContent.Add(streamContent, "file", "manifest.json");
        multipartContent.Add(new StringContent(avatarJson, Encoding.UTF8, "application/json"), "AvatarDto");

        HttpResponseMessage res = await client.PostAsync("/modpacks/import/curseforge", multipartContent);

        Assert.AreEqual(HttpStatusCode.Created, res.StatusCode);

        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        Modpack? importedModpack = await secondContext.Modpacks.SingleOrDefaultAsync(m => m.UserId == user.Id);

        Assert.IsNotNull(importedModpack);
    }

    [TestMethod]
    public async Task DownloadModpackManifestTest()
    {
        var context = Services.GetRequiredService<PackbuilderContext>();
        var modpackFactory = Services.GetRequiredService<ModpackFactory>();
        var userFactory = Services.GetRequiredService<UserFactory>();
        var modFactory = Services.GetRequiredService<ModFactory>();

        (User user, string password) = userFactory.CreateUser(true);
        Modpack modpack = modpackFactory.CreateModpack(user, Game.Minecraft);
        Mod mod = modFactory.CreateMod();
        ModpackVersion modpackVersion = modpack.CreateVersionFromMods([mod], "1.20", ModLoader.Forge);

        modpack.Slug = Modpack.GenerateSlug(modpack.Name);

        context.Users.Add(user);
        context.Modpacks.Add(modpack);
        context.Mods.Add(mod);
        context.Versions.Add(modpackVersion);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"/modpacks/{modpack.Id}/download/version/{modpackVersion.Iteration}");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.AreEqual("application/zip", res.Content.Headers.ContentType?.MediaType);

        byte[] zipBytes = await res.Content.ReadAsByteArrayAsync();

        using MemoryStream stream = new(zipBytes);
        using ZipArchive archive = new(stream, ZipArchiveMode.Read);

        ZipArchiveEntry? manifestEntry =
            archive.GetEntry("manifest.json");

        Assert.IsNotNull(manifestEntry);

        using Stream manifestStream = manifestEntry.Open();

        CurseForgeManifest? manifest = await JsonSerializer.DeserializeAsync<CurseForgeManifest>(manifestStream);

        Assert.IsNotNull(manifest);
    }
}

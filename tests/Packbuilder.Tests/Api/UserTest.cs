using Packbuilder.Tests.Factories;
using Packbuilder.Models;
using Packbuilder.Dto.Create;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Bogus;
using System.Net.Http.Json;
using Packbuilder.Dto.Update;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Models.enums;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace Packbuilder.Tests.Api;

[TestClass]
public class UserTest : ApplicationTests
{
    [ClassInitialize]
    public static void Setup(TestContext ctx)
    {
        BaseSetup(ctx);
    }

    [TestMethod]
    public async Task GetUserTest()
    {
        var userFactory = Services.GetRequiredService<UserFactory>();
        var context = Services.GetRequiredService<PackbuilderContext>();

        (User user, string password) = userFactory.CreateUser(true);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"/users/{user.Id}");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    [TestMethod]
    public async Task CreateUserTest() {
        Faker faker = new();
        PackbuilderWebApplicationFactory application = new(TestConnectionString);
        HttpClient client = application.CreateClient();
        
        HttpResponseMessage res = await client.PostAsJsonAsync("/users", new CreateUserDto
        {
            Name = faker.Name.FirstName(),
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password(),
            ImageType = ImageType.Stock,
            ImageValue = "profile_avatar_1.jpg"
        });

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    [TestMethod]
    public async Task UpdateUserTest()
    {
        var userFactory = Services.GetRequiredService<UserFactory>();
        var context = Services.GetRequiredService<PackbuilderContext>();

        Faker faker = new();
        (User user, string password) = userFactory.CreateUser(true);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        UpdateUserDto updateUserDto = new()
        {
            Name = faker.Name.FirstName(),
            ImageValue = "profile_avatar_2.jpg",
            ImageType = ImageType.Stock
        };

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.PutAsJsonAsync($"/users/{user.Name}", updateUserDto);

        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        User? updatedUser = await secondContext.Users.SingleOrDefaultAsync(u => user.Id == u.Id);

        Assert.IsNotNull(updatedUser);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.AreNotEqual(user.Name, updatedUser.Name);
        Assert.AreNotEqual(user.ImageValue, updatedUser.ImageValue);
    }

    [TestMethod]
    public async Task DeleteUserTest() {
        var userFactory = Services.GetRequiredService<UserFactory>();
        var context = Services.GetRequiredService<PackbuilderContext>();

        (User user, string password) = userFactory.CreateUser(true);
        context.Users.Add(user);
        await context.SaveChangesAsync();

        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.DeleteAsync($"/users/{user.Name}");

        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        User? deletedUser = await secondContext.Users.SingleOrDefaultAsync(u => user.Id == u.Id);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.IsNull(deletedUser);
    }

    // TODO: Add tests for updating password/email as the requirements and systems in place for doing so have changed.
}

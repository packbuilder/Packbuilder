using Packbuilder.Tests.Factories;
using Packbuilder.Models;
using Packbuilder.Dto.Create;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using Bogus;
using System.Net.Http.Json;
using Packbuilder.Dto.Update;
using Microsoft.EntityFrameworkCore;

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

        (User user, string password) = userFactory.CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();
        HttpClient client = await CreateSessionClient(user, password);
        HttpResponseMessage res = await client.GetAsync($"/users/{user.Id}");

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    [TestMethod]
    public async Task CreateUserTest() {
        Faker faker = new();
        PackbuilderWebApplicationFactory application = new();
        HttpClient client = application.CreateClient();
        JsonContent data = JsonContent.Create(new CreateUserDto
        {
            Name = faker.Name.FirstName(),
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password()
        });
        HttpResponseMessage res = await client.PostAsync("/users", data);

        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
    }

    [TestMethod]
    public async Task UpdateUserTest()
    {
        var userFactory = Services.GetRequiredService<UserFactory>();
        var context = Services.GetRequiredService<PackbuilderContext>();

        Faker faker = new();
        (User user, string password) = userFactory.CreateUser();
        context.Users.Add(user);
        await context.SaveChangesAsync();

        UpdateUserDto updateUserDto = new UpdateUserDto
        {
            Name = faker.Name.FirstName(),
            Email = faker.Internet.Email(),
            Password = faker.Internet.Password()
        };

        HttpClient client = await CreateSessionClient(user, password);
        JsonContent data = JsonContent.Create(updateUserDto);
        HttpResponseMessage res = await client.PutAsync($"/users/{user.Name}", data);

        var scope = Services.CreateScope();
        var secondContext = scope.ServiceProvider.GetRequiredService<PackbuilderContext>();
        User? updatedUser = await secondContext.Users.SingleOrDefaultAsync(u => user.Id == u.Id);

        Assert.IsNotNull(updatedUser);
        Assert.AreEqual(HttpStatusCode.OK, res.StatusCode);
        Assert.AreNotEqual(user.Name, updatedUser.Name);
        Assert.AreNotEqual(user.Email, updatedUser.Email);
        Assert.AreNotEqual(user.PasswordDigest, updatedUser.PasswordDigest);
        
    }

    [TestMethod]
    public async Task DeleteUserTest() {
        var userFactory = Services.GetRequiredService<UserFactory>();
        var context = Services.GetRequiredService<PackbuilderContext>();

        (User user, string password) = userFactory.CreateUser();
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
}

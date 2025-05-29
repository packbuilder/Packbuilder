using Bogus;
using Microsoft.AspNetCore.Identity;
using Packbuilder.Models;

namespace Packbuilder.Tests.Factories;

public class UserFactory(IPasswordHasher<User> passwordHasher)
{
    public (User, string) CreateUser()
    {
        Faker faker = new();
        string name = NameFactory.Create();
        string password = faker.Internet.Password();

        User user = new Faker<User>()
            .RuleFor(u => u.Name, (f, u) => f.Name.FirstName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email())
            .RuleFor(u => u.Avatar, (f, u) => f.Internet.Avatar());

        user.SetPassword(passwordHasher, password);

        return (user, password);
    }
}
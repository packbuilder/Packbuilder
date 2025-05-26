using Bogus;
using Microsoft.AspNetCore.Identity;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Tests.Factories;

public class UserFactory(IPasswordHasher<User> passwordHasher)
{
    public (User, string) CreateUser(bool isVerified)
    {
        Faker faker = new();
        string name = NameFactory.Create();
        string password = faker.Internet.Password();


        User user = new Faker<User>()
            .RuleFor(u => u.Name, (f, u) => f.Name.FirstName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email())
            .RuleFor(u => u.ImageType, (f, u) => ImageType.Stock)
            .RuleFor(u => u.ImageValue, (f, u) => "profile_avatar_1.jpg")
            .RuleFor(u => u.EmailVerified, (f, u) => isVerified);
            
        user.SetPassword(passwordHasher, password);

        return (user, password);
    }
}
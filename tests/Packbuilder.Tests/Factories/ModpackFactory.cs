using Bogus;
using NuGet.Protocol.Plugins;
using Packbuilder.Models;

namespace Packbuilder.Tests.Factories;

public class ModpackFactory
{
    public Modpack CreateModpack(User user)
    {
        Faker faker = new();
        string name = NameFactory.Create();

        Modpack modpack = new Faker<Modpack>()
                .RuleFor(m => m.Name, (f, m) => name)
                .RuleFor(m => m.Slug, (f, m) => Modpack.GenerateSlug(name))
                .RuleFor(m => m.UserId, (f, m) => user.Id)
                .RuleFor(m => m.User, (f, m) => user)
                .RuleFor(m => m.Avatar, (f, m) => f.Internet.Avatar());

        return modpack;
    }
}
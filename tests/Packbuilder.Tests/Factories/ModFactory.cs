using Bogus;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Tests.Factories;

public class ModFactory
{
    public Mod CreateMod()
    {
        Faker faker = new();
        string name = NameFactory.Create();

        Enum platform = Platform.Thunderstore;

        Mod mod = new Faker<Mod>()
                .RuleFor(m => m.Platform, (f, m) => platform)
                .RuleFor(m => m.ReferenceId, (f, m) => $"thunderstore{name}");

        return mod;
    }
}
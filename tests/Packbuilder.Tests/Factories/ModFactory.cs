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

        ModPlatform platform = ModPlatform.CurseForge;

        Mod mod = new Faker<Mod>()
                .RuleFor(m => m.Platform, (f, m) => platform)
                .RuleFor(m => m.Name, (f, m) => name)
                .RuleFor(m => m.ReferenceId, (f, m) => $"curseForge{name}");

        return mod;
    }
}
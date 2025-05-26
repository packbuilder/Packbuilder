using Bogus;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Tests.Factories;

public partial class SuggestionFactory
{
    public Suggestion CreateSuggestion(User user, Modpack modpack, ModpackVersion modpackVersion)
    {
        Faker faker = new();
        string memo = faker.Lorem.Paragraph();

        Suggestion suggestion = new Faker<Suggestion>()
                .RuleFor(s => s.Memo, (f, s) => memo)
                .RuleFor(s => s.VersionId, (f, s) => modpackVersion.Id)
                .RuleFor(s => s.VersionIteration, (f, s) => modpackVersion.Iteration)
                .RuleFor(s => s.ModpackId, (f, s) => modpack.Id)
                .RuleFor(s => s.User, (f, s) => user)
                .RuleFor(s => s.UserId, (f, s) => user.Id)
                .RuleFor(s => s.State, (f, s) => SuggestionState.Unverified)
                .RuleFor(s => s.ModLoader, (f, s) => ModLoader.Forge)
                .RuleFor(s => s.GameVersion, (f, s) => "1.20")
                .RuleFor(s => s.Modifications, (f, s) => [])
                .RuleFor(s => s.Modpack, (f, s) => modpack);

        return suggestion;
    }
}
using Bogus;
using Packbuilder.Models;

namespace Packbuilder.Tests.Factories;

public partial class SuggestionFactory
{
    public Suggestion CreateSuggestion(User user, Modpack modpack)
    {
        Faker faker = new();
        string memo = faker.Lorem.Paragraph();

        Suggestion suggestion = new Faker<Suggestion>()
                .RuleFor(s => s.Memo, (f, s) => memo)
                .RuleFor(s => s.ModpackSlug, (f, s) => Modpack.GenerateSlug(modpack.Name))
                .RuleFor(s => s.Username, (f, s) => user.Name)
                .RuleFor(s => s.User, (f, s) => user)
                .RuleFor(s => s.Modpack, (f, s) => modpack);

        return suggestion;
    }
}
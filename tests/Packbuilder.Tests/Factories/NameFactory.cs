using System.Text.RegularExpressions;
using Bogus;

namespace Packbuilder.Tests.Factories;

public partial class NameFactory
{
    public static string Create()
    {
        Faker faker = new();
        var first = faker.Name.FirstName();
        var suffix = faker.Random.Int(1, 100000);

        return IllegalCharacters().Replace($"{first}_{suffix}".ToLower(), "");
    }

    [GeneratedRegex(@"[^A-Za-z0-9_]")]
    private static partial Regex IllegalCharacters();
}
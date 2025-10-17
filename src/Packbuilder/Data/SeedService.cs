using Microsoft.AspNetCore.Identity;
using Packbuilder.Models;

namespace Packbuilder.Data;

public class SeedService(PackbuilderContext db, IPasswordHasher<User> passwordHasher)
{
    public async Task Seed()
    {
        User user = new()
        {
            Name = "Goob",
            Email = "balls@poopmail.com",
            Avatar = "../Seed-Avatar.jpg",
        };

        user.SetPassword(passwordHasher, "password");

        Modpack modpack = new()
        {
            Name = "The Woah",
            Avatar = "../modpack.gif",
            Slug = Modpack.GenerateSlug("The Woah"),
            UserId = user.Id,
            User = user
        };

        db.Users.Add(user);
        db.Modpacks.Add(modpack);
        await db.SaveChangesAsync();
    }

    private static string Encode(byte[] bytes, string type) => $"data:image/{type};base64,{Convert.ToBase64String(bytes)}";
}
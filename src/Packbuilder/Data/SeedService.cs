using System.Data;
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

        Mod mod1 = new()
        {
            Platform = Platform.CurseForge,
            ReferenceId = "238222"
        };

        Mod mod2 = new()
        {
            Platform = Platform.CurseForge,
            ReferenceId = "348521"
        };

        List<Mod> mods = [mod1, mod2];

        ModpackVersion version = modpack.CreateVersion(mods);

        db.Users.Add(user);
        db.Modpacks.Add(modpack);
        db.Versions.Add(version);
        foreach (VersionMod versionMod in version.VersionMods)
        {
            db.VersionMods.Add(versionMod);
        }
        await db.SaveChangesAsync();
    }

    private static string Encode(byte[] bytes, string type) => $"data:image/{type};base64,{Convert.ToBase64String(bytes)}";
}
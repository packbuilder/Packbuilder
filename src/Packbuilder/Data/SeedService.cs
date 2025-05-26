using Microsoft.AspNetCore.Identity;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Data;

public class SeedService(PackbuilderContext db, IPasswordHasher<User> passwordHasher)
{
    public async Task Seed()
    {
        User user = new()
        {
            Name = "Goob",
            Email = "solizg02@gmail.com",
            ImageValue = "profile_avatar_1.jpg",
            ImageType = ImageType.Stock
        };

        user.SetPassword(passwordHasher, "passwords");

        Modpack modpack = new()
        {
            Name = "TheWoah",
            ImageValue = "modpack_avatar_1.gif",
            ImageType = ImageType.Stock,
            Slug = Modpack.GenerateSlug("TheWoah"),
            UserId = user.Id,
            User = user
        };

        Mod mod1 = new()
        {
            Platform = ModPlatform.CurseForge,
            ReferenceId = "238222"
        };

        Mod mod2 = new()
        {
            Platform = ModPlatform.CurseForge,
            ReferenceId = "348521"
        };

        List<Mod> mods = [mod1, mod2];

        ModpackVersion version = modpack.CreateVersionFromMods(mods, "1.20", ModLoader.Forge);

        db.Users.Add(user);
        db.Modpacks.Add(modpack);
        db.Versions.Add(version);
        foreach (VersionMod versionMod in version.VersionMods)
        {
            db.VersionMods.Add(versionMod);
        }
        await db.SaveChangesAsync();
    }
}
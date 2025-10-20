using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;

namespace Packbuilder.Models
{
    [Table("modpack")]
    public partial class Modpack : ModelBase
    {
        [MaxLength(20)]
        [Column("name")]
        public required string Name { get; set; }
        [Column("avatar")]
        public string? Avatar { get; set; }
        [Column("slug")]
        public required string Slug { get; set; }
        [Column("user_id")]
        public required int UserId { get; set; }
        public ICollection<ModpackVersion> Versions { get; set; } = [];

        public User User { get; set; } = null!;

        public ModpackVersion CreateVersion(List<Mod> mods, Modpack curModpack)
        {
            float latest = 0.0f;
            foreach (ModpackVersion modpackVersion in Versions)
            {
                if (latest < modpackVersion.Iteration)
                {
                    latest = modpackVersion.Iteration;
                }
            }

            ModpackVersion newVersion = new()
            {
                ModpackId = Id,
                Iteration = latest + .1f,
                Modpack = curModpack,
                VersionMods = []
            };

            foreach (Mod mod in mods)
            {
                VersionMod versionMod = new()
                {
                    ModId = mod.Id,
                    VersionIteration = latest + .1f,
                    ModpackId = curModpack.Id,
                    Version = newVersion,
                    Mod = mod
                };

                newVersion.VersionMods.Add(versionMod);
            }

            Versions.Add(newVersion);

            return newVersion;
        }

        public static string GenerateSlug(string name)
        {
            string slug = SlugRegex().Replace(name, "");
            slug = slug.Trim().Replace(' ', '-').ToLower();

            return slug;
        }

        [GeneratedRegex(@"[^a-z0-9-]")]
        private static partial Regex SlugRegex();
    }
}
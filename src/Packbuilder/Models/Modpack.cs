using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.RegularExpressions;
using Packbuilder.Models.enums;
using Packbuilder.Validators;

namespace Packbuilder.Models
{
    [Table("modpack")]
    public partial class Modpack : ModelBase
    {
        [Required]
        [Name]
        [Column("name")]
        public required string Name { get; set; }
        [Required]
        [Column("image_type")]
        public required ImageType ImageType { get; set; }
        [Required]
        [Column("image_value")]
        public required string ImageValue { get; set; }
        [Required]
        [Column("slug")]
        public required string Slug { get; set; }
        [Required]
        [Column("user_id")]
        public required int UserId { get; set; }
        public ICollection<ModpackVersion> Versions { get; set; } = [];
        public User User { get; set; } = null!;

        public static Modpack CreateModpack(string name, User currentUser, ImageType imageType, string imageValue)
        {
            return new Modpack()
            {
                Name = name,
                UserId = currentUser.Id,
                Slug = Modpack.GenerateSlug(name + currentUser.Name),
                ImageType = imageType,
                ImageValue = imageValue
            };
        }

        public ModpackVersion CreateVersionFromModifications(List<Modification> modifications, List<VersionMod> existingMods, string gameVersion, ModLoader modLoader)
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
                GameVersion = gameVersion,
                ModLoader = modLoader,
                Modpack = this,
                VersionMods = []
            };

            newVersion.VersionMods = existingMods.Select(vm => new VersionMod()
            {
                ModId = vm.ModId,
                ModpackId = Id,
                VersionIteration = newVersion.Iteration,
                Version = newVersion,
                Mod = vm.Mod,
                ConflictState = vm.ConflictState
            }).ToList();

            List<VersionMod> modsToAdd = modifications
                .Where(m => m.ModAction == ModAction.Added).Select(modification => new VersionMod()
                {
                    ModId = modification.Mod.Id,
                    VersionIteration = newVersion.Iteration,
                    ModpackId = Id,
                    Modpack = this,
                    Version = newVersion,
                    Mod = modification.Mod,
                    ConflictState = modification.ConflictState
                }).ToList();

            HashSet<int> removedModIds = modifications
                .Where(m => m.ModAction == ModAction.Removed)
                .Select(m => m.Mod.Id)
                .ToHashSet();

            List<VersionMod> modsToRemove= newVersion.VersionMods
                .Where(vm => removedModIds.Contains(vm.Mod.Id))
                .ToList();

            foreach (VersionMod versionMod in modsToAdd)
            {
                newVersion.VersionMods.Add(versionMod);
            }

            foreach (VersionMod versionMod in modsToRemove)
            {
                newVersion.VersionMods.Remove(versionMod);
            }

            Versions.Add(newVersion);

            return newVersion;
        }

        public ModpackVersion CreateVersionFromMods(List<Mod> mods, string gameVersion, ModLoader modLoader)
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
                GameVersion = gameVersion,
                ModLoader = modLoader,
                Modpack = this,
                VersionMods = []
            };

            foreach (Mod mod in mods)
            {
                VersionMod versionMod = new()
                {
                    ModId = mod.Id,
                    VersionIteration = latest + .1f,
                    ModpackId = Id,
                    Modpack = this,
                    Version = newVersion,
                    Mod = mod,
                    ConflictState = ConflictState.NoConflicts
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
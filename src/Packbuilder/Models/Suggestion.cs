using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Packbuilder.Dto.Create;
using Packbuilder.Models.enums;
using Packbuilder.Validators;


namespace Packbuilder.Models
{
    [Table("suggestions")]
    public class Suggestion : ModelBase
    {
        [Column("modpack_id")]
        public required int ModpackId { get; set; }
        [Column("user_id")] 
        public required int UserId { get; set; }
        [Required]
        [Memo]
        [Column("memo")]
        public required string Memo { get; set; }
        [Required]
        [Column("version_iteration")]
        public required float VersionIteration { get; set; }
        [Required]
        [Column("version_id")]
        public required int VersionId { get; set; }
        [Required]
        [GameVersion]
        [Column("gameVersion")]
        public required string GameVersion { get; set; }
        [Required]
        [Column("modLoader")]
        public required ModLoader ModLoader { get; set; }
        [Required]
        [Column("state")]
        public SuggestionState State { get; set; } = SuggestionState.Unverified;
        public virtual Modpack Modpack { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public ICollection<Modification> Modifications { get; set; } = [];

        public static Suggestion CreateSuggestion(Modpack modpack, User user, ModpackVersion version, CreateSuggestionDto body)
        {
            return new()
            {
                ModpackId = modpack.Id,
                UserId = user.Id,
                Memo = body.Memo,
                Modpack = modpack,
                GameVersion = body.GameVersion,
                ModLoader = body.ModLoader,
                VersionIteration = version.Iteration,
                VersionId = version.Id,
                User = user
            };
        }
 
        public Modification CreateModification(Mod mod, ModAction modAction)
        {
            Modification newModification = new() {
                ModId = mod.Id,
                ModAction = modAction,
                SuggestionId = this.Id,
                ConflictState = ConflictState.NoConflicts,
                Mod = mod,
                Suggestion = this
            };

            Modifications.Add(newModification);

            return newModification;
        }
    }
}
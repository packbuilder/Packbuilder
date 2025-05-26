using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Packbuilder.Models.enums;
using Packbuilder.Validators;

namespace Packbuilder.Models
{
    [Table("versions")]
    public class ModpackVersion : ModelBase
    {
        [Required]
        [Column("modpack_id")]
        public required int ModpackId { get; set; }
        [Required]
        [Column("iterations")]
        public required float Iteration { get; set; }
        [Required]
        [GameVersion]
        [Column("game_version")]
        public required string GameVersion { get; set; }
        [Required]
        [Column("mod_loader")]
        public required ModLoader ModLoader { get; set; }
        public virtual Modpack Modpack { get; set; } = null!;
        public virtual ICollection<VersionMod> VersionMods { get; set; } = [];
    }
}
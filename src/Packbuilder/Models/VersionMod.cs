using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Packbuilder.Models.enums;

namespace Packbuilder.Models
{
    [Table("version_mods")]
    public class VersionMod : ModelBase
    {
        [Required]
        [Column("mod_id")]
        public required int ModId { get; set; }
        [Required]
        [Column("modpack_id")]
        public required int ModpackId { get; set; }
        [Required]
        [Column("version_iteration")]
        public required float VersionIteration { get; set; }
        [Required]
        [Column("conflict_state")]
        public required ConflictState ConflictState { get; set; } = ConflictState.NoConflicts;
        public virtual ModpackVersion Version { get; set; } = null!;
        public virtual Modpack Modpack { get; set; } = null!;
        public virtual Mod Mod { get; set; } = null!;
    }
}
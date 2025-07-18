using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("version_mods")]
    public class VersionMod : ModelBase
    {
        [Column("mod_id")]
        public required int ModId { get; set; }
        [Column("version_iteration")]
        public required float VersionIteration { get; set; }
        public virtual ModpackVersion Version { get; set; } = null!;
        public virtual Mod Mod { get; set; } = null!;
    }
}
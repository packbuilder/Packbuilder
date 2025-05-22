using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("version_mods")]
    public class VersionMod : BaseModel
    {
        [Column("mod")]
        public required Mod Mod { get; set; }
        [Column("version")]
        public required Version Version { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("modpack_mods")]
    public class ModpackMods : BaseModel
    {
        [Column("mod")]
        public required Mod Mod { get; set; }
        [Column("modpack")]
        public required Modpack Modpack { get; set; }
    }
}
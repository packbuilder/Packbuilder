using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("versions")]
    public class Version : ModelBase
    {
        [Column("modpack_id")]
        public required int ModpackId { get; set; }
        [Column("iterations")]
        public required float Iteration { get; set; }
        public virtual Modpack Modpack { get; set; } = null!;
        public virtual ICollection<VersionMod> VersionMods { get; set; } = null!;
    }
}
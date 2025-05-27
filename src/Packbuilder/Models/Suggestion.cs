using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("suggestions")]
    public class Suggestion : ModelBase
    {
        [Column("modpack_slug")]
        public required string ModpackSlug { get; set; }
        [Column("username")]
        public required string Username { get; set; }
        [Column("memo")]
        public required string Memo { get; set; }
        public virtual Modpack Modpack { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
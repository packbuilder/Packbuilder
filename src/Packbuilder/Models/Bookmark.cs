using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    public class Bookmark : ModelBase
    {
        [Required]
        [Column("user_id")]
        public required int UserId { get; set; }
        [Required]
        [Column("modpack_id")]
        public required int ModpackId { get; set; }
        public virtual Modpack Modpack { get; set; } = null!;
    }
}
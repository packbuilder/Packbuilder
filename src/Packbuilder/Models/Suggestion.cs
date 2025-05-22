using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("suggestions")]
    public class Suggestion : BaseModel
    {
        [Column("modpack")]
        public required Modpack Modpack { get; set; }
        [Column("user")]
        public required User User { get; set; }
        [Column("memo")]
        public required string Memo { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("modifications")]
    public class Modification : BaseModel
    {
        [Column("mod")]
        public required Mod Mod { get; set; }
        [Column("action")]
        public required Action Action { get; set; }
        [Column("suggestion")]
        public required Suggestion Suggestion { get; set; }
    }
}
using System.ComponentModel.DataAnnotations.Schema;

namespace Packbuilder.Models
{
    [Table("mods")]
    public class Mod : ModelBase
    {
        [Column("platform")]
        public required Platform Platform { get; set; }
        [Column("reference_id")]
        public required string ReferenceId { get; set; }
    }
}
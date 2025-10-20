using System.ComponentModel.DataAnnotations.Schema;
using Packbuilder.Models.enums;

namespace Packbuilder.Models
{
    [Table("mods")]
    public class Mod : ModelBase
    {
        [Column("platform")]
        public required ModPlatform Platform { get; set; }
        [Column("reference_id")]
        public required string ReferenceId { get; set; }
    }
}
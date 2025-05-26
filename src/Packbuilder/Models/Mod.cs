using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.CodeAnalysis;

namespace Packbuilder.Models
{
    [Table("mods")]
    public class Mod : BaseModel
    {
        [Column("platform")]
        public required Platform Platform { get; set; }
        [Column("reference_id")]
        public required string ReferenceId { get; set; }
    }
}
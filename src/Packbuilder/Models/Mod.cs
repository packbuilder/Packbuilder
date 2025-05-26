using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Models.enums;

namespace Packbuilder.Models
{
    [Table("mods")]
    [Index(nameof(ReferenceId), IsUnique = true)]
    public class Mod : ModelBase
    {
        [Required]
        [Column("platform")]
        public required ModPlatform Platform { get; set; }
        [Required]
        [Column("reference_id")]
        public required string ReferenceId { get; set; }
    }
}
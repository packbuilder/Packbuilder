using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Packbuilder.Models.enums;

namespace Packbuilder.Models
{
    [Table("modifications")]
    public class Modification : ModelBase
    {
        [Required]
        [Column("mod_id")]
        public required int ModId { get; set; }
        [Required]
        [Column("action")]
        public required ModAction ModAction { get; set; }
        [Required]
        [Column("suggestion_id")]
        public required int SuggestionId { get; set; }
        [Required]
        [Column("conflict_state")]
        public required ConflictState ConflictState { get; set; } = ConflictState.NoConflicts;
        public virtual Mod Mod { get; set; } = null!;
        public virtual Suggestion Suggestion { get; set; } = null!;
    }
}
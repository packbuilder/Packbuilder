using System.ComponentModel.DataAnnotations.Schema;
using Packbuilder.Models.enums;

namespace Packbuilder.Models
{
    [Table("modifications")]
    public class Modification : ModelBase
    {
        [Column("mod_id")]
        public required int ModId { get; set; }
        [Column("action")]
        public required ModAction ModAction { get; set; }
        [Column("suggestion_id")]
        public required int SuggestionId { get; set; }
        public virtual Mod Mod { get; set; } = null!;
        public virtual Suggestion Suggestion { get; set; } = null!;
    }
}
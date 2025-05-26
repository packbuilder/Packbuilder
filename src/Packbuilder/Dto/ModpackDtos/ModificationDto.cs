using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class ModificationDto : ModificationPartialDto
    {
        
        [JsonPropertyName("mod")] 
        public virtual Mod Mod { get; set; } = null!;
        [JsonPropertyName("suggestion")]
        public virtual SuggestionPartialDto Suggestion { get; set; } = null!;

        [SetsRequiredMembers]
        public ModificationDto(Modification modification)
        {
            Id = modification.Id;
            ModId = modification.ModId;
            State = modification.ConflictState;
            ModAction = modification.ModAction;
            SuggestionId = modification.SuggestionId;
            Mod = modification.Mod;
        }
    }
}
using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class ModificationPartialDto
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("modId")]
        public required int ModId { get; set; }
        [JsonPropertyName("conflictState")]
        public required ConflictState State { get; set; }
        [JsonPropertyName("modAction")]
        public required ModAction ModAction { get; set; }
        [JsonPropertyName("suggestionId")]
        public required int SuggestionId { get; set; }
    }
}
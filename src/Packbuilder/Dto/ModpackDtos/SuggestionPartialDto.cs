using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class SuggestionPartialDto
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("modpackId")]
        public required int ModpackId { get; set; }
        [JsonPropertyName("userId")] 
        public required int UserId { get; set; }
        [JsonPropertyName("memo")]
        public required string Memo { get; set; }
        [JsonPropertyName("gameVersion")]
        public required string GameVersion { get; set; }
        [JsonPropertyName("modLoader")]
        public required ModLoader ModLoader { get; set; }
        [JsonPropertyName("versionIteration")]
        public required float VersionIteration { get; set; }
        [JsonPropertyName("versionId")]
        public required int VersionId { get; set; }
        [JsonPropertyName("state")]
        public SuggestionState State { get; set; } = SuggestionState.Unverified;
    }
}
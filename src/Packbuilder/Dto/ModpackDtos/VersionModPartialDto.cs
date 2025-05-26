using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class VersionModPartialDto
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("modId")]
        public required int ModId { get; set; }
        [JsonPropertyName("versionIteration")]
        public required float VersionIteration { get; set; }
        [JsonPropertyName("conflictState")]
        public required ConflictState ConflictState { get; set; }
    }
}
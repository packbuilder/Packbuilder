using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class VersionPartialDto
    
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("modpackId")]
        public required int ModpackId { get; set; }
        [JsonPropertyName("iterations")]
        public float Iteration { get; set; }
        [JsonPropertyName("gameVersion")]
        public required string GameVersion { get; set; }
        [JsonPropertyName("modLoader")]
        public required ModLoader ModLoader { get; set; }
        [JsonPropertyName("modpack")]
        public virtual ModpackPartialDto ModpackPartialDto { get; set; } = null!;
    }
}
using System.Text.Json.Serialization;
using CurseForge.enums;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeModLoaderSummary
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("gameVersion")]
        public required string GameVersion { get; set; }
        [JsonPropertyName("latest")]
        public required bool Latest { get; set; }
        [JsonPropertyName("recommended")]
        public required bool Recommended { get; set; }
        [JsonPropertyName("type")]
        public required MinecraftModLoader Type { get; set; }
    }
}
using System.Text.Json.Serialization;
using Packbuilder.Dto.CurseForgeDtos;

namespace CurseForge.Dtos.ManifestDtos
{
    public class CurseForgeManifestResponseDto
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("minecraft")]
        public required MinecraftInfoDto MinecraftInfo { get; set; }
        [JsonPropertyName("files")]
        public required List<CurseForgeManifestModDto> ModFiles { get; set; }
    }
}
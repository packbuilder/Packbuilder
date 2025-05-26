using System.Text.Json.Serialization;

namespace Packbuilder.Dto.CurseForgeDtos
{
    public class MinecraftInfoDto
    {
        [JsonPropertyName("version")]
        public required string GameVersion { get; set; }
        [JsonPropertyName("modLoaders")]
        public required List<ModLoaderManifest> ModLoaders { get; set; }
    }   
}
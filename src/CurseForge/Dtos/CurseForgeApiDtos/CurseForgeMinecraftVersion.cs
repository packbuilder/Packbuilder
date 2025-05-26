using System.Text.Json.Serialization;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeMinecraftVersion
    {   
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("versionString")]
        public required string Name { get; set; }
    }
}
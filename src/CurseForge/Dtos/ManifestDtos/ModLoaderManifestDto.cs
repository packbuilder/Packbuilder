using System.Text.Json.Serialization;

namespace Packbuilder.Dto.CurseForgeDtos
{
    public class ModLoaderManifest
    {
        [JsonPropertyName("id")]
        public required string Id { get; set; }
        [JsonPropertyName("primary")]
        public required bool IsPrimary { get; set; }
    }
}
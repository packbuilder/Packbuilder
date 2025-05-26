using System.Text.Json.Serialization;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeModLinks
    {
        [JsonPropertyName("websiteUrl")]
        public required string WebsiteUrl { get; set; }
        [JsonPropertyName("sourceUrl")]
        public required string SourceUrl { get; set; }
    }
}
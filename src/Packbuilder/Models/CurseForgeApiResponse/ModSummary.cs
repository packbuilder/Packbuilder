using System.Text.Json.Serialization;

namespace Packbuilder.Models.CurseForgeApiResponse
{
    public class CurseForgeModSummary
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
        [JsonPropertyName("logo")]
        public CurseForgeModLogo? Logo { get; set; }
    }
}
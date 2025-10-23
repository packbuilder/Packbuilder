using System.Text.Json.Serialization;

namespace CurseForge.Models;

public class CurseForgeModLogo
{
    [JsonPropertyName("id")]
    public required int Id { get; set; }
    [JsonPropertyName("thumbnailUrl")]
    public required string ThumbnailUrl { get; set; }
    [JsonPropertyName("url")]
    public required string Url { get; set; }
}

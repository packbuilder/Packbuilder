using System.Text.Json.Serialization;

namespace CurseForge.Dtos.ManifestDtos
{
    public class CurseForgeManifestModDto
    {
        [JsonPropertyName("projectID")]
        public required int ProjectId { get; set; }
        [JsonPropertyName("fileID")]
        public required int FileId { get; set; }
        [JsonPropertyName("required")]
        public bool? IsRequired { get; set; }
    }
}
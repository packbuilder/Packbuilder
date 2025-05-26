using System.Text.Json.Serialization;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeManifestMod
    {
        [JsonPropertyName("projectID")]
        public required int ProjectId { get; set; }
        [JsonPropertyName("fileID")]
        public required int FileId { get; set; }
    }
}
using System.Text.Json.Serialization;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeFile
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("gameVersions")]
        public required List<string> GameVersions { get; set; }
        [JsonPropertyName("modId")]
        public required int ModReferenceId { get; set; }
        [JsonPropertyName("fileName")]
        public required string FileName { get; set; }
        [JsonPropertyName("fileDate")]
        public required string FileDate { get; set; }
        [JsonPropertyName("dependencies")]
        public required List<CurseForgeModDependency> Dependencies { get; set;} = [];
    }
}
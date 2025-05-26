using System.Text.Json.Serialization;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeModSummary
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("summary")]
        public required string Summary { get; set; }
        [JsonPropertyName("downloadCount")]
        public required int DownloadCount { get; set; }
        [JsonPropertyName("dateModified")]
        public required string DateModified { get; set; }
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
        [JsonPropertyName("links")]
        public required CurseForgeModLinks Links { get; set; }
        [JsonPropertyName("authors")]
        public required List<CurseForgeAuthor> Authors { get; set; }
        [JsonPropertyName("latestFiles")]
        public required List<CurseForgeFile> ModFiles { get; set; }
        [JsonPropertyName("logo")]
        public CurseForgeModLogo? Logo { get; set; }
        [JsonPropertyName("classId")]
        public required int ClassId { get; set; }
    }
}
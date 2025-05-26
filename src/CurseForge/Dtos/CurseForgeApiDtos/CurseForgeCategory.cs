using System.Text.Json.Serialization;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeCategory
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
        [JsonPropertyName("classId")]
        public required int ClassId { get; set; }
    }
}
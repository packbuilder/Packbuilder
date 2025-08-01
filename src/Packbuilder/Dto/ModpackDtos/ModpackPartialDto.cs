using System.Text.Json.Serialization;

namespace Packbuilder.Dto.ModpackDtos
{
    public class ModpackPartialDto
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
        [JsonPropertyName("user_id")]
        public required int UserId { get; set; }
    }
}
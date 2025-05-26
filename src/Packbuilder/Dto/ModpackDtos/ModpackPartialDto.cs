using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class ModpackPartialDto
    {
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("imageType")]
        public required ImageType ImageType { get; set; }
        [JsonPropertyName("imageValue")]
        public required string ImageValue { get; set; }
        [JsonPropertyName("slug")]
        public required string Slug { get; set; }
        [JsonPropertyName("userId")]
        public required int UserId { get; set; }
    }
}
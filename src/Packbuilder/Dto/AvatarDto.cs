using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto
{
    public class AvatarDto
    {
        [JsonPropertyName("imageValue")]
        public required string ImageValue { get; set; }
        [JsonPropertyName("imageType")]
        public required ImageType ImageType { get; set; }
    }
}
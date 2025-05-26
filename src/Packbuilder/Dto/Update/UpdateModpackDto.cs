using System.Text.Json.Serialization;
using Packbuilder.Models.enums;
using Packbuilder.Validators;

namespace Packbuilder.Dto.Update
{
    public class UpdateModpackDto
    {
        [Name]
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("imageType")]
        public ImageType? ImageType { get; set; }
        [JsonPropertyName("imageValue")]
        public string? ImageValue { get; set; }
    }
}
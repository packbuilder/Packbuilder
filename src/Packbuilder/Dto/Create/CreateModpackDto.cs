using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Models.enums;
using Packbuilder.Validators;

namespace Packbuilder.Dto.Create
{
    public class CreateModpackDto
    {
        [Required]
        [Name]
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [Required]
        [GameVersion]
        [JsonPropertyName("gameVersion")]
        public required string GameVersion { get; set; }
        [Required]
        [JsonPropertyName("modLoader")]
        public required ModLoader ModLoader { get; set; }
        [Required]
        [JsonPropertyName("imageType")]
        public required ImageType ImageType { get; set; }
        [Required]
        [JsonPropertyName("imageValue")]
        public required string ImageValue { get; set; }

        //TODO: Add game id here if I ever decide to support more games
    }
}
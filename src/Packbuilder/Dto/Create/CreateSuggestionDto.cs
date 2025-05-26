using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Models.enums;
using Packbuilder.Validators;

namespace Packbuilder.Dto.Create
{
    public class CreateSuggestionDto
    {
        [Required]
        [Memo]
        [JsonPropertyName("memo")]
        public required string Memo { get; set; }
        [Required]
        [GameVersion]
        [JsonPropertyName("gameVersion")]
        public required string GameVersion { get; set; }
        [Required]
        [JsonPropertyName("modLoader")]
        public required ModLoader ModLoader { get; set; }
    }
}
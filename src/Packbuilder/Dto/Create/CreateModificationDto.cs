using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.Create
{
    public class CreateModificationDto {
        [Required]
        [JsonPropertyName("modReferenceId")]
        public required string ModReferenceId { get; set; }
        [Required]
        [JsonPropertyName("modPlatform")]
        public required ModPlatform ModPlatform { get; set; }
        [Required]
        [JsonPropertyName("modAction")]
        public required ModAction ModAction { get; set; }
    }
}
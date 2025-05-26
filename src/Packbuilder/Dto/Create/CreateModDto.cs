using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.Create
{
    public class CreateModDto
    {
        [Required]
        [JsonPropertyName("platform")]
        public required ModPlatform Platform { get; set; }
        [Required]
        [JsonPropertyName("referenceId")]
        public required string ReferenceId { get; set; }
    }
}
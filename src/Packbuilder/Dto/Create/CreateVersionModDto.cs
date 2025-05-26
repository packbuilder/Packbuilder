using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.Create
{
    public class CreateVersionModDto
    {
        [Required]
        [JsonPropertyName("iteration")]
        public required float Iteration { get; set; }
        [Required]
        [JsonPropertyName("modpackId")]
        public required int ModpackId { get; set; }
        [Required]
        [JsonPropertyName("modId")]
        public required int ModId { get; set; }
    }
}
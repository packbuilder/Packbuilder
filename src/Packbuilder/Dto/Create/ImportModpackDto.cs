using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto
{
    public class ImportModpackDto
    {
        [Required]
        [JsonPropertyName("file")]
        public required IFormFile File { get; set; }
        [Required]
        [JsonPropertyName("avatarDto")]
        public required AvatarDto AvatarDto { get; set; }
    }
}
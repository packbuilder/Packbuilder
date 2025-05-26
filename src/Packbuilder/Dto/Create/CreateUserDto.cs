using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.Create
{
    public class CreateUserDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(20)]
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [Required]
        [MinLength(8)]
        [MaxLength(64)]
        [JsonPropertyName("password")]
        public required string Password { get; set; }
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [JsonPropertyName("imageType")]
        public required ImageType ImageType { get; set; }
        [JsonPropertyName("imageValue")]
        public required string ImageValue { get; set; }
    }
}
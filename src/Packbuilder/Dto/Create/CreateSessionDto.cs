using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Validators;

namespace Packbuilder.Dto.Create
{
    public class CreateSessionDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [Required]
        [Password]
        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
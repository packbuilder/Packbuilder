using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Validators;

namespace Packbuilder.Dto.Update
{
    public class ResetEmailDto
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("newEmail")]
        public required string NewEmail { get; set; }
        [Required]
        [Password]
        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
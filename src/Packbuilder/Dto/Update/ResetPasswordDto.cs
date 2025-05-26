using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Packbuilder.Validators;

namespace Packbuilder.Dto.Update
{
    public class ResetPasswordDto
    {
        [Required]
        [Password]
        [JsonPropertyName("newPassword")]
        public required string NewPassword { get; set; }
        [Required]
        [JsonPropertyName("token")]
        public required string Token { get; set; }
    }
}
using System.Text.Json.Serialization;
using Packbuilder.Models.enums;
using Packbuilder.Validators;

namespace Packbuilder.Dto
{
    public class UserDto
    {
        [Name]
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("id")]
        public required int Id { get; set; }
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [JsonPropertyName("emailVerified")]
        public required bool EmailVerified { get; set; }
        [JsonPropertyName("imageType")]
        public required ImageType ImageType { get; set; }
        [JsonPropertyName("imageValue")]
        public required string ImageValue { get; set; }
    }
}
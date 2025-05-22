using System.Text.Json.Serialization;

namespace Packbuilder.Dto
{
    public class CreateUserDto
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        [JsonPropertyName("password")]
        public required string Password { get; set; }
        [JsonPropertyName("email")]
        public required string Email { get; set; }
    }
}
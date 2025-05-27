using System.Text.Json.Serialization;

namespace Packbuilder.Dto.Create
{
    public class CreateSessionDto
    {
        [JsonPropertyName("email")]
        public required string Email { get; set; }
        [JsonPropertyName("password")]
        public required string Password { get; set; }
    }
}
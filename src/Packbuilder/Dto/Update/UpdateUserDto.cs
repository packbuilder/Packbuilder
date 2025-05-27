using System.Text.Json.Serialization;

namespace Packbuilder.Dto.Update
{
    public class UpdateUserDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;
        [JsonPropertyName("password")]
        public string Password { get; set; } = null!;
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;
    }
}
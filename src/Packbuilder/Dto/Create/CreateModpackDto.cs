using System.Text.Json.Serialization;

namespace Packbuilder.Dto.Create
{
    public class CreateModpackDto
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
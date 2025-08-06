using System.Text.Json.Serialization;

namespace Packbuilder.Dto.Create
{
    public class CreateModpackVersionDto
    {
        [JsonPropertyName("modpack_id")]
        public required int ModpackId { get; set; }
        [JsonPropertyName("iteration")]
        public required float Iteration { get; set; }
    }
}
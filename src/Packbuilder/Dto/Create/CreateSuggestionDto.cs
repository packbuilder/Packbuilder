using System.Text.Json.Serialization;

namespace Packbuilder.Dto.Create
{
    public class CreateSuggestionDto
    {
        [JsonPropertyName("memo")]
        public required string Memo { get; set; }
    }
}
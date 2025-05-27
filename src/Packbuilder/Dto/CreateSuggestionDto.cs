using System.Text.Json.Serialization;

namespace Packbuilder.Dto
{
    public class CreateSuggestionDto
    {
        [JsonPropertyName("memo")]
        public required string Memo { get; set; }
    }
}
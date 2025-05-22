using System.Text.Json.Serialization;
using Packbuilder.Models;

namespace Packbuilder.Dto
{
    public class CreateModpackDto
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }
        //Deep dive on that avatar shit brotha
    }
}
using System.Text.Json.Serialization;
using Packbuilder.Models;

namespace Packbuilder.Dto.ServiceDtos
{
    public class RedisModDataDto<T>
    {
        [JsonPropertyName("modId")]
        public required string ModId { get; set; }
        [JsonPropertyName("modData")]
        public required T ModData { get; set; }
    }
}
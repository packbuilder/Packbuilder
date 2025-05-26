using System.Text.Json.Serialization;

namespace Packbuilder.Dto.ServiceDtos
{
    public class ModCacheDto<T>
    {
        [JsonPropertyName("cachedMods")]
        public List<T> CachedMods { get; set; } = [];
        [JsonPropertyName("uncachedMods")]
        public List<string> UncachedModIds { get; set; } = [];
    }
}
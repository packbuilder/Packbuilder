using Newtonsoft.Json;

namespace Packbuilder.Dto.ModpackDtos
{
    public class VersionPartialDto
    
    {
        [JsonProperty("id")]
        public required int Id { get; set; }
        [JsonProperty("modpack_id")]
        public required int ModpackId { get; set; }
        [JsonProperty("iterations")]
        public float Iteration { get; set; }
        [JsonProperty("modpack")]
        public virtual ModpackPartialDto ModpackPartialDto { get; set; } = null!;
    }
}
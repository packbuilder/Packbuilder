using Newtonsoft.Json;

namespace Packbuilder.Dto.ModpackDtos
{
    public class ModpackVersionPartialDto
    
    {
        [JsonProperty("modpack_id")]
        public required int ModpackId { get; set; }
        [JsonProperty("iterations")]
        public float Iteration { get; set; }
        [JsonProperty("modpack")]
        public virtual ModpackPartialDto ModpackPartialDto { get; set; } = null!;
    }
}
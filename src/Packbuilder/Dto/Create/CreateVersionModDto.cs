using Newtonsoft.Json;

namespace Packbuilder.Dto.Create
{
    public class CreateVersionModDto
    {
        [JsonProperty("iteration")]
        public required float Iteration { get; set; }
        [JsonProperty("modpack_id")]
        public required int ModpackId { get; set; }
        [JsonProperty("mod_id")]
        public required int ModId { get; set; }
    }
}
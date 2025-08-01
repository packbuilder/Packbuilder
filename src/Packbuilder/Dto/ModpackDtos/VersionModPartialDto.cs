using Newtonsoft.Json;

namespace Packbuilder.Dto.ModpackDtos
{
    public class VersionModPartialDto
    {
        [JsonProperty("mod_id")]
        public required int ModId { get; set; }
        [JsonProperty("version_iteration")]
        public required float VersionIteration { get; set; }
    }
}
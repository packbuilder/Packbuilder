using Newtonsoft.Json;

namespace Packbuilder.Dto.Create
{
    public class CreateVersionDto
    {
        [JsonProperty("mod_ids")]
        public required List<int> ModIds { get; set; }
    }
}
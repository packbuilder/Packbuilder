using Newtonsoft.Json;

namespace Packbuilder.Dto.Update
{
    public class UpdateModpackDto
    {
        [JsonProperty("name")]
        public string Name { get; set; } = null!;
        [JsonProperty("avatar")]
        public string Avatar { get; set; } = null!;
    }
}
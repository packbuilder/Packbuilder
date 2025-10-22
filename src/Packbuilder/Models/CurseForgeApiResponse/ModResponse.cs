using Newtonsoft.Json;

namespace Packbuilder.Models.CurseForgeApiResponse
{    
    public class CurseForgeModResponse
    {
        [JsonProperty("data")]
        public CurseForgeModSummary? Data { get; set; }
    }
}
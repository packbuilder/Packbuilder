using Newtonsoft.Json;

namespace Packbuilder.Models.CurseForgeApiResponse
{    
    public class CurseForgeModListResponse
    {
        [JsonProperty("data")]
        public List<CurseForgeModSummary>? Data { get; set; }
        [JsonProperty("pagination")]
        public required CurseForgePagination Pagination { get; set; }
    }
}

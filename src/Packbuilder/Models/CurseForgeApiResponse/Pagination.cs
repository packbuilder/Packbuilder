using Newtonsoft.Json;

namespace Packbuilder.Models.CurseForgeApiResponse
{
    public class CurseForgePagination
    {
        [JsonProperty("index")]
        public required int Index { get; set; }
        [JsonProperty("pageSize")]
        public required int PageSize { get; set; }
        [JsonProperty("resultCount")]
        public required int ResultCount { get; set; }
        [JsonProperty("totalCount")]
        public required int TotalCount { get; set; }
    }
}
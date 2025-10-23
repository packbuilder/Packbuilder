using System.Text.Json.Serialization;

namespace CurseForge.Models
{
    public class CurseForgePagination
    {
        [JsonPropertyName("index")]
        public required int Index { get; set; }
        [JsonPropertyName("pageSize")]
        public required int PageSize { get; set; }
        [JsonPropertyName("resultCount")]
        public required int ResultCount { get; set; }
        [JsonPropertyName("totalCount")]
        public required int TotalCount { get; set; }
    }
}
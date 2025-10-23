using System.Text.Json.Serialization;

namespace CurseForge.Models
{    
    public class CurseForgeModListResponse
    {
        [JsonPropertyName("data")]
        public List<CurseForgeModSummary>? Data { get; set; }
        [JsonPropertyName("pagination")]
        public required CurseForgePagination Pagination { get; set; }
    }
}

namespace CurseForge.Models
{    
    public class CurseForgeModListResponse
    {
        [JsonProperty("data")]
        public List<CurseForgeModSummary>? Data { get; set; }
        [JsonProperty("pagination")]
        public required CurseForgePagination Pagination { get; set; }
    }
}

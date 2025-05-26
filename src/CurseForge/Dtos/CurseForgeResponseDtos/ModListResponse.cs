using System.Text.Json.Serialization;
using CurseForge.Dtos.CurseForgeApiDtos;

namespace CurseForge.Dtos.CurseForgeResponseDtos
{    
    public class CurseForgeModListResponse
    {
        [JsonPropertyName("data")]
        public List<CurseForgeModSummary>? Data { get; set; }
        [JsonPropertyName("pagination")]
        public CurseForgePagination? Pagination { get; set; }
    }
}

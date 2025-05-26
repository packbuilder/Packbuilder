using System.Text.Json.Serialization;
using CurseForge.Dtos.CurseForgeApiDtos;

namespace CurseForge.Dtos.CurseForgeResponseDtos
{
    public class CurseForgeModLoadersResponse
    {
        [JsonPropertyName("data")]
        public List<CurseForgeModLoaderSummary>? Data { get; set; }    
    }
}
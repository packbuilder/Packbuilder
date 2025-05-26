using System.Text.Json.Serialization;
using CurseForge.Dtos.CurseForgeApiDtos;

namespace CurseForge.Dtos.CurseForgeResponseDtos
{    
    public class CurseForgeModResponse
    {
        [JsonPropertyName("data")]
        public CurseForgeModSummary? Data { get; set; }
    }
}
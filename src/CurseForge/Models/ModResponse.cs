using System.Text.Json.Serialization;

namespace CurseForge.Models
{    
    public class CurseForgeModResponse
    {
        [JsonPropertyName("data")]
        public CurseForgeModSummary? Data { get; set; }
    }
}
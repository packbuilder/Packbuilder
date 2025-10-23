namespace CurseForge.Models
{    
    public class CurseForgeModResponse
    {
        [JsonProperty("data")]
        public CurseForgeModSummary? Data { get; set; }
    }
}
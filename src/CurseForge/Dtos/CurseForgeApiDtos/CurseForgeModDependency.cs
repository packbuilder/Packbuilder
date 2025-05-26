using System.Text.Json.Serialization;

namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeModDependency
    {   
        [JsonPropertyName("modId")]
        public required int ModReferenceId { get; set; }
        [JsonPropertyName("relationType")]
        public required int RelationType { get; set; }
    }
}
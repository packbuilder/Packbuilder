using System.Text.Json.Serialization;
using CurseForge.Dtos.CurseForgeApiDtos;

namespace CurseForge.Dtos.CurseForgeResponseDtos
{    
    public class CurseForgeCategoryResponse
    {
        [JsonPropertyName("data")]
        public List<CurseForgeCategory>? Data { get; set; }
    }
}

using System.Text.Json.Serialization;
using CurseForge.Dtos.CurseForgeApiDtos;

namespace CurseForge.Dtos.CurseForgeResponseDtos
{    
    public class CurseForgeFilesResponse
    {
        [JsonPropertyName("data")]
        public List<CurseForgeFile>? Data { get; set; }
    }
}

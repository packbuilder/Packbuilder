using System.Text.Json.Serialization;
using CurseForge.Dtos.CurseForgeApiDtos;

namespace CurseForge.Dtos.CurseForgeResponseDtos
{    
    public class MinecraftVersionResponse
    {
        [JsonPropertyName("data")]
        public List<CurseForgeMinecraftVersion>? Data { get; set; }
    }
}

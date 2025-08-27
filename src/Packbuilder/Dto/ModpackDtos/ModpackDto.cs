using System.Text.Json.Serialization;
using Packbuilder.Models;

namespace Packbuilder.Dto.ModpackDtos
{
    public class ModpackDto : ModpackPartialDto
    {
        [JsonPropertyName("versions")] public ICollection<VersionDto> Versions { get; set; }  

  
        public ModpackDto(Modpack modpack)
        {
            Versions = [.. modpack.Versions.Select(m => new VersionDto(m, m.Id, 0.1f))];
        }
    }
}
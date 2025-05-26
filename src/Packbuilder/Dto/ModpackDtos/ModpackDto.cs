using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Packbuilder.Models;

namespace Packbuilder.Dto.ModpackDtos
{
    public class ModpackDto : ModpackPartialDto
    {
        [JsonPropertyName("versions")] public ICollection<VersionDto> Versions { get; set; }  
        [JsonPropertyName("user")] public User? User { get; set; }

        [SetsRequiredMembers]
        public ModpackDto(Modpack modpack)
        {
            Id = modpack.Id;
            Slug = modpack.Slug;
            Name = modpack.Name;
            UserId = modpack.UserId;
            User = modpack.User;
            ImageType = modpack.ImageType;
            ImageValue = modpack.ImageValue;
            Versions = [.. modpack.Versions.Select(v => new VersionDto(v))];
        }
    }
}
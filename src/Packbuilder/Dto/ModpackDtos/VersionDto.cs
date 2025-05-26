using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Dto.ModpackDtos
{
    public class VersionDto : VersionPartialDto
    {
        [JsonPropertyName("versionMods")]
        public virtual ICollection<VersionModDto> VersionMods { get; set; } = [];

        [SetsRequiredMembers]
        public VersionDto(ModpackVersion version)
        {
            ModpackId = version.ModpackId;
            Iteration = version.Iteration;
            Id = version.Id;
            GameVersion = version.GameVersion;
            ModLoader = version.ModLoader;
            VersionMods = [.. version.VersionMods.Select(v => new VersionModDto(v){
                ModId = v.ModId,
                VersionIteration = v.VersionIteration
            })];
        }
    }
}
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Packbuilder.Models;

namespace Packbuilder.Dto.ModpackDtos
{

    public class VersionModDto : VersionModPartialDto
    {
        [JsonProperty("version")]
        public virtual VersionDto VersionDto { get; set; } = null!;
        [JsonProperty("mod")]
        public virtual Mod Mod { get; set; } = null!;

        [SetsRequiredMembers]
        public VersionModDto(VersionMod versionMod)
        {
            ModId = versionMod.Id;
            VersionIteration = versionMod.VersionIteration;
        }
    }
}
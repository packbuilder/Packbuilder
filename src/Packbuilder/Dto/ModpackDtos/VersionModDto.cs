using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Packbuilder.Models;

namespace Packbuilder.Dto.ModpackDtos
{

    public class VersionModDto : VersionModPartialDto
    {
        [JsonPropertyName("version")]
        public virtual VersionPartialDto VersionPartialDto { get; set; } = null!;
        [JsonPropertyName("mod")]
        public virtual Mod? Mod { get; set; } = null!;

        [SetsRequiredMembers]
        public VersionModDto(VersionMod versionMod)
        {
            ModId = versionMod.Id;
            Mod = versionMod.Mod;
            VersionIteration = versionMod.VersionIteration;
            ConflictState = versionMod.ConflictState;
        }
    }
}
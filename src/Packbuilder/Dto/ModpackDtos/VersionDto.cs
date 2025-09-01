using System.Diagnostics.CodeAnalysis;
using Packbuilder.Models;

namespace Packbuilder.Dto.ModpackDtos
{
    public class VersionDto : VersionPartialDto
    {
        public virtual ICollection<VersionModPartialDto> VersionMods { get; set; } = [];

        [SetsRequiredMembers]
        public VersionDto(ModpackVersion modpackVersion, int modpackId, float iteration)
        {
            ModpackId = modpackId;
            Iteration = iteration;
            Id = modpackVersion.Id;
            VersionMods = [.. modpackVersion.VersionMods.Select(v => new VersionModPartialDto{
                ModId = v.ModId,
                VersionIteration = v.VersionIteration
            })];
        }
    }
}
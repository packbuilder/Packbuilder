using System.Diagnostics.CodeAnalysis;
using Packbuilder.Models;

namespace Packbuilder.Dto.ModpackDtos
{
    public class ModpackVersionDto : ModpackVersionPartialDto
    {
        public virtual ICollection<VersionModDto> VersionMods { get; set; } = [];

        [SetsRequiredMembers]
        public ModpackVersionDto(ModpackVersion modpackVersion, int modpackId, float iteration)
        {
            ModpackId = modpackId;
            Iteration = iteration;
            VersionMods = [.. modpackVersion.VersionMods.Select(v => new VersionModDto(v))];
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Services
{
    public class VersionModService(PackbuilderContext context) : IVersionModService
    {
        public async Task<VersionModDto?> FindVersionMod(float versionIteration, int modId, int modpackId)
        {
            VersionMod? versionMod = await context.VersionMods.Include(v => v.Mod).SingleOrDefaultAsync(v => v.ModId == modId && v.VersionIteration == versionIteration && v.ModpackId == modpackId);

            if (versionMod is null) return null;

            return new VersionModDto(versionMod);
        }
    }
}
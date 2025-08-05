using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Services
{
    public class ModpackVersionService(PackbuilderContext context) : IModpackVersionService
    {
        public async Task<ModpackVersionDto?> GetCurrentVersion(int modpackId)
        {
            ModpackVersion? modpackVersion = await context.Versions.SingleOrDefaultAsync(v => v.ModpackId == modpackId);


            if (modpackVersion is null)
            {
                return null;
            }

            return new ModpackVersionDto(modpackVersion, modpackId, modpackVersion.Iteration);
        }

        public async Task<ModpackVersionDto?> FindVersion(int modpackId, float iteration)
        {
            ModpackVersion? modpackVersion = await context.Versions.SingleOrDefaultAsync(v => v.ModpackId == modpackId && v.Iteration == iteration);

            if (modpackVersion is null)
            {
                return null;
            }

            return new ModpackVersionDto(modpackVersion, modpackId, iteration);
        }

        public Task<ModpackVersionDto?> UpdateModpackVersion()
        {
            return null;
        }
    }
}
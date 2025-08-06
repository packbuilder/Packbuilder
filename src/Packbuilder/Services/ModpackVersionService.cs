using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Services
{
    public class ModpackVersionService(PackbuilderContext context) : IModpackVersionService
    {
        public async Task<ModpackVersionDto?> GetLatestVersion(int modpackId)
        {
            //This function will get the requested version
            Modpack? modpack = await context.Modpacks.Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1)).SingleOrDefaultAsync(m => m.Id == modpackId);


            if (modpack is null)
            {
                return null;
            }

            ModpackVersion latestVersion = modpack.Versions.First();

            return new ModpackVersionDto(latestVersion, modpackId, latestVersion.Iteration);
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
    }
}
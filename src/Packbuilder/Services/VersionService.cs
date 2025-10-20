using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Services
{
    public class VersionService(PackbuilderContext context) : IVersionService
    {
        public async Task<VersionDto?> GetLatestVersion(int modpackId)
        {
            //This function will get the requested version
            Modpack? modpack = await context.Modpacks.Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1)).SingleOrDefaultAsync(m => m.Id == modpackId);


            if (modpack is null)
            {
                return null;
            }

            ModpackVersion latestVersion = modpack.Versions.First();

            return new VersionDto(latestVersion, modpackId, latestVersion.Iteration);
        }

        public async Task<VersionDto?> FindVersion(int modpackId, float iteration)
        {
            ModpackVersion? modpackVersion = await context.Versions.SingleOrDefaultAsync(v => v.ModpackId == modpackId && v.Iteration == iteration);

            if (modpackVersion is null)
            {
                return null;
            }

            return new VersionDto(modpackVersion, modpackId, iteration);
        }

        public async Task<VersionDto?> CreateVersion(int modpackId, List<int> modIds)
        {
            Modpack? currentModpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId);
            List<Mod> mods = [];

            if (currentModpack is null)
            {
                return null;
            }

            foreach (int id in modIds)
            {
                Mod? mod = await context.Mods.SingleOrDefaultAsync(m => id == m.Id);

                if (mod is not null)
                {
                    mods.Add(mod);
                }
            }

            ModpackVersion newVersion = currentModpack.CreateVersion(mods, currentModpack);

            foreach (VersionMod versionMod in newVersion.VersionMods)
            {
                context.VersionMods.Add(versionMod);
            }
            
            context.Versions.Add(newVersion);
            await context.SaveChangesAsync();

            return new VersionDto(newVersion, currentModpack.Id, newVersion.Iteration);
        }
    }
}
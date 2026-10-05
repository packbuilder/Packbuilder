using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.Dtos.ManifestDtos;
using CurseForge.enums;
using CurseForge.Interfaces;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class CurseForgeModpackService(PackbuilderContext context, ICurseForgeManifestService curseForgeManifestService, IModService modService) : ICurseForgeModpackService
    {
        public async Task ImportMinecraftModpackAsync(CurseForgeManifestDto manifestDto, List<ExternalModSummary> modSummaries, int userId, AvatarDto body)
        {
            User user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId) ?? throw new Exception("Could not import modpack because user does not exist");

            ModLoader modLoader = MinecraftModLoaderMapper.TryConvertFromMinecraftModLoader(manifestDto.ModLoader, out bool isSupported);

            if(!isSupported)
            {
                throw new Exception("This manifest is not using a supported mod loader");
            }

            Modpack modpack = Modpack.CreateModpack(manifestDto.Name, user, body.ImageType, body.ImageValue, Game.Minecraft);

            context.Modpacks.Add(modpack);

            List<Mod> modpackMods = [];

            foreach (ExternalModSummary modSummary in modSummaries)
            {
                Mod mod = await modService.GetOrCreateModAsync(modSummary);
            }

            ModpackVersion modpackVersion = modpack.CreateVersionFromMods(modpackMods, manifestDto.GameVersion, modLoader);

            context.Versions.Add(modpackVersion);

            foreach (VersionMod versionMod in modpackVersion.VersionMods)
            {
                context.VersionMods.Add(versionMod);
            }

            await context.SaveChangesAsync();
            
            return;
        }

        public async Task<MemoryStream> CreateMinecraftModpackManifest(int modpackId, float versionIteration)
        {
            Modpack? modpack = await context.Modpacks
                .Include(m => m.User).Include(m => m.Versions.Where(v => v.Iteration == versionIteration))
                    .SingleOrDefaultAsync(m => m.Id == modpackId) ?? throw new Exception($"Could not get manifest.json for modpack with id {modpackId} because it doesn't exist");  

            ModpackVersion selectedVersion = modpack.Versions.First() 
                ?? throw new Exception($"Version {versionIteration} does not exist for modpack with id {modpackId}");

            MinecraftModLoader minecraftModLoader = MinecraftModLoaderMapper.TryConvertToMinecraftModLoader(selectedVersion.ModLoader, out bool isSupported);

            if(!isSupported) throw new Exception($"Modpack {modpackId} version {versionIteration} is not using a known minecraft launcher");

            List<string>? modReferenceIds = await context.VersionMods.Include(v => v.Mod).Where(v => v.ModpackId == modpackId && v.VersionIteration == versionIteration).Select(v => v.Mod.ReferenceId).ToListAsync();

            MemoryStream manifestZip = await curseForgeManifestService.CreateManifestZipFile(modReferenceIds, selectedVersion.GameVersion, minecraftModLoader, modpack.Name, versionIteration.ToString(), modpack.User.Name);

            return manifestZip;
        }
    }
}

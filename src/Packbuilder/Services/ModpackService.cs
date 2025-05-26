using CurseForge.Dtos.ManifestDtos;
using CurseForge.enums;
using CurseForge.Interfaces;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class ModpackService(PackbuilderContext context, ICurseForgeManifestService curseForgeManifestService) : IModpackService
    {
        public async Task CreateModpackAsync(CreateModpackDto body, int userId)
        {
            User user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId) ?? throw new Exception("Could not create modpack because user does not exist");

            List<Modpack> userModpacks = await context.Modpacks.Where(m => m.UserId == user.Id).ToListAsync();

            if(userModpacks.Count == 20) throw new Exception("User has already created the maximum amount of modpacks.");

            if(userModpacks.Find(m => m.Name == body.Name) is not null) throw new Exception($"This user has already made a modpack with the name {body.Name}");

            Modpack modpack = Modpack.CreateModpack(body.Name, user, body.ImageType, body.ImageValue);
            ModpackVersion modpackVersion = modpack.CreateVersionFromMods([], body.GameVersion, body.ModLoader); 

            context.Modpacks.Add(modpack);
            context.Versions.Add(modpackVersion);
            await context.SaveChangesAsync();

            return;
        }

        public async Task ImportCurseForgeModpackAsync(CurseForgeManifestDto manifestDto, int userId, AvatarDto body)
        {
            User user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId) ?? throw new Exception("Could not import modpack because user does not exist");

            ModLoader modLoader = MinecraftModLoaderMapper.TryConvertFromMinecraftModLoader(manifestDto.ModLoader, out bool isSupported);

            List<string> modReferenceIds = manifestDto.ModFiles.Select(m => m.ProjectId.ToString()).ToList();

            if(!isSupported)
            {
                throw new Exception("This manifest is not using a supported mod loader");
            }

            Modpack modpack = Modpack.CreateModpack(manifestDto.Name, user, body.ImageType, body.ImageValue);

            context.Modpacks.Add(modpack);

            List<Mod> modpackMods = [];

            foreach (string referenceId in modReferenceIds)
            {
                Mod? mod = await context.Mods.SingleOrDefaultAsync(m => m.ReferenceId == referenceId);

                if(mod is null)
                {
                    mod = new()
                    {
                        ReferenceId = referenceId,
                        Platform = ModPlatform.CurseForge
                    };

                    context.Mods.Add(mod);
                }

                modpackMods.Add(mod);
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

        public async Task<MemoryStream> GetCurseForgeModpackManifest(int modpackId, float versionIteration)
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

        public async Task UpdateModpackAsync(UpdateModpackDto body, int modpackId)
        {
            Modpack? modpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId)
                ?? throw new Exception($"Could not update modpack with id {modpackId} because it doesn't exist.");
            
            if(body.Name is not null && body.Name != null) modpack.Name = body.Name;

            if(body.ImageValue is not null && body.ImageValue != null)
            {
                modpack.ImageValue = body.ImageValue;
            }

            if(body.ImageType is { } imageType)
            {
                modpack.ImageType = imageType;    
            }

            context.Modpacks.Update(modpack);
            await context.SaveChangesAsync();

            return;
        }

        public async Task DeleteModpackAsync(int modpackId)
        {
            Modpack? modpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId)
                ?? throw new Exception($"Could not delete modpack with id {modpackId} because it doesn't exist.");
            
            context.RemoveRange(modpack);
            await context.SaveChangesAsync();

            return;
        }

    }
}


using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.Interfaces;
using Packbuilder.Dto.ServiceDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class CurseForgeModService(ICacheService cacheService, ICurseForgeApiService curseForgeApiService) : ICurseForgeModService
    {
        public async Task<CurseForgeMod> GetModWithCacheAsync(string referenceId)
        {
            CurseForgeMod? cachedModData = await cacheService.GetModAsync<CurseForgeMod>(referenceId, ModPlatform.CurseForge);

            if(cachedModData is not null)
            {   
                return cachedModData;
            }

            CurseForgeMod? curseForgeMod = await curseForgeApiService.GetModAsync(referenceId.ToString()) ?? throw new Exception("Could not retrieve CurseForge mod.");

            RedisModDataDto<CurseForgeMod> modDataDto = new()
            {
                ModId = curseForgeMod.ReferenceId,
                ModData = curseForgeMod
            };

            await cacheService.SetModsAsync([modDataDto], ModPlatform.CurseForge);

            return curseForgeMod;
        }
    
        public async Task<List<CurseForgeMod>> GetModsWithCacheAsync(List<string> referenceIds)
        {
            ModCacheDto<CurseForgeMod>? modCacheDto = await cacheService.GetModsAsync<CurseForgeMod>(referenceIds, ModPlatform.CurseForge);

            if(modCacheDto.UncachedModIds.Count <= 0)
            {
                return modCacheDto.CachedMods;
            }

            List<CurseForgeMod>? curseForgeMods = await curseForgeApiService.GetModsAsync(modCacheDto.UncachedModIds) ?? throw new Exception("Could not retrieve CurseForge mods");

            List<RedisModDataDto<CurseForgeMod>> modDataDtos = curseForgeMods.Select(mod => new RedisModDataDto<CurseForgeMod>
            {
                ModId = mod.ReferenceId,
                ModData = mod
            }).ToList();

            await cacheService.SetModsAsync(modDataDtos, ModPlatform.CurseForge);

            List<CurseForgeMod> mods = [..modCacheDto.CachedMods, ..curseForgeMods];

            return mods;
        }

    }
}

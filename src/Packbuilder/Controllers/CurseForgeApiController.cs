using Microsoft.AspNetCore.Mvc;
using CurseForge.Interfaces;
using Packbuilder.Models.enums;
using Packbuilder.Dto.ServiceDtos;
using Packbuilder.Interfaces;
using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.enums;
using Packbuilder.Attributes;
using Packbuilder.RateLimits;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("curseforge")]
    public class CurseForgeApiController(ICurseForgeApiService curseForgeApiService, ICacheService cacheService) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.Curseforge)]
        [HttpGet("{referenceId}")]
        [EndpointName("GetMod")]
        public async Task<ActionResult<CurseForgeMod>> GetMod([FromRoute] string referenceId)
        {
            CurseForgeMod? cachedData = await cacheService.GetModAsync<CurseForgeMod>(referenceId, ModPlatform.CurseForge);

            if(cachedData is not null)
            {   
                Console.WriteLine("This is cached data");
                return Ok(cachedData);
            }

            CurseForgeMod? curseForgeMod = await curseForgeApiService.GetModAsync(referenceId.ToString());

            if (curseForgeMod is null)
            {
                return NotFound(null);
            }

            RedisModDataDto<CurseForgeMod> modDataDto = new()
            {
                ModId = curseForgeMod.ReferenceId,
                ModData = curseForgeMod
            };

            await cacheService.SetModsAsync([modDataDto], ModPlatform.CurseForge);

            return Ok(curseForgeMod);
        }

        [RateLimit(RateLimitBuckets.Curseforge)]
        [HttpGet("search/{gameId}")]
        [EndpointName("SearchMods")]
        public async Task<ActionResult<CurseForgeModList?>> SearchMods([FromRoute] int gameId, [FromQuery] string searchQuery, [FromQuery] int index, [FromQuery] int pageSize, [FromQuery] ModSortField sortField, [FromQuery] string gameVersion, [FromQuery] ModLoader modLoader)
        {
            MinecraftModLoader minecraftModLoader = MinecraftModLoaderMapper.TryConvertToMinecraftModLoader(modLoader, out bool isSupported);

            if(searchQuery.Length <= 0 || !isSupported)
            {
                return BadRequest();
            }

            string searchPayload = $"{gameId}/{searchQuery}/{index}/{pageSize}/{sortField}/{gameVersion}/{modLoader}";

            index *= pageSize;

            CurseForgeModList? cachedData = await cacheService.GetPaginationDataAsync<CurseForgeModList>(searchPayload, ModPlatform.CurseForge); 

            if(cachedData is not null)
            {
                Console.WriteLine("This is cached data");
                return Ok(cachedData);
            }

            CurseForgeModList? curseForgeModList = await curseForgeApiService.SearchModsAsync(gameId, index, pageSize, searchQuery, gameVersion, minecraftModLoader, sortField);

            if (curseForgeModList is null)
            {
                return NotFound(null);
            }

            await cacheService.SetPaginationDataAsync(curseForgeModList, searchPayload, ModPlatform.CurseForge);

            return Ok(curseForgeModList);
        }

        [RateLimit(RateLimitBuckets.Curseforge)]
        [HttpGet("minecraft/versions")]
        [EndpointName("GetMinecraftVersions")]
        public async Task<ActionResult<List<string>>> GetMinecraftVersions()
        {
            List<string>? minecraftVersions = await curseForgeApiService.GetMinecraftVersions();

            if(minecraftVersions is null)
            {
                return Problem(
                    detail: "Unable to fetch minecraft versions from curseforge",
                    title: "Internal Server Error",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }

            return Ok(minecraftVersions);
        }

        // Add pagination. This endpoint will now take in a pagination parameter along with the entire list of modIds?
        [RateLimit(RateLimitBuckets.Curseforge)]
        [HttpPost]
        [EndpointName("GetModsCurseForge")]
        public async Task<ActionResult<List<CurseForgeMod>?>> GetMods([FromBody] List<string> modIds)
        {
            if(modIds.Count <= 0)
            {
                return BadRequest([]);
            }

            ModCacheDto<CurseForgeMod>? modCacheDto = await cacheService.GetModsAsync<CurseForgeMod>(modIds, ModPlatform.CurseForge);

            if(modCacheDto.UncachedModIds.Count <= 0)
            {
                Console.WriteLine("This is cached data");
                return Ok(modCacheDto.CachedMods);
            }

            List<CurseForgeMod>? curseForgeMods = await curseForgeApiService.GetModsAsync(modCacheDto.UncachedModIds);

            if (curseForgeMods is null)
            {
                return NotFound(null);
            }

            List<RedisModDataDto<CurseForgeMod>> modDataDtos = curseForgeMods.Select(mod => new RedisModDataDto<CurseForgeMod>
            {
                ModId = mod.ReferenceId,
                ModData = mod
            }).ToList();

            await cacheService.SetModsAsync(modDataDtos, ModPlatform.CurseForge);

            List<CurseForgeMod> mods = [..modCacheDto.CachedMods, ..curseForgeMods];

            return Ok(mods);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Attributes;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Models;
using Packbuilder.RateLimits;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("/modpacks/{modpackId}/versions/{iteration}/mods")]
    public class VersionModsController(PackbuilderContext context) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet]
        [EndpointName("GetModsPackbuilder")]
        public async Task<ActionResult<PaginatedResponse<VersionMod>>> GetVersionMods([FromRoute] float iteration, [FromRoute] int modpackId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if(page < 1 || pageSize < 1)
            {
                return BadRequest();
            }

            pageSize = Math.Min(pageSize, 50);

            Modpack? curModpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId);

            if(curModpack is null)
            {
                return NotFound();
            }
            
            IQueryable<VersionMod> query = context.VersionMods.Where(v => v.VersionIteration == iteration && v.ModpackId == curModpack.Id);

            int totalItems = await query.CountAsync();
            int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
 
            List<VersionMod> versionMods = await query.Include(v => v.Mod)
                    .Skip((page - 1) * pageSize)
                        .Take(pageSize).ToListAsync();
            
            if (versionMods is null)
            {
                return NotFound();
            }

            List<VersionModDto> versionModDtos = [.. versionMods.Select(v => new VersionModDto(v))];

            return Ok(new PaginatedResponse<VersionModDto> { 
                Items = versionModDtos,
                Page = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
            });
        }
    }
}
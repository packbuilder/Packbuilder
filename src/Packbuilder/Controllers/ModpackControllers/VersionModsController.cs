using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Attributes;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.RateLimits;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("/modpacks/{modpackId}/versions/{iteration}/mods")]
    public class VersionModsController(PackbuilderContext context, IPaginationService paginationService) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet]
        [EndpointName("GetModsPackbuilder")]
        public async Task<ActionResult<PaginatedResponse<VersionMod>>> GetVersionMods([FromRoute] float iteration, [FromRoute] int modpackId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = "")
        {
            Modpack? curModpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId);

            if(curModpack is null)
            {
                return NotFound();
            }
            
            IQueryable<VersionMod> query = context.VersionMods.Where(v => v.VersionIteration == iteration && v.ModpackId == curModpack.Id && v.Mod.Name.Contains(searchQuery)).Include(v => v.Mod);

            PaginatedResponse<VersionMod> paginatedResponse = await paginationService.GetPaginatedData(query, page, pageSize); 

            return Ok(new PaginatedResponse<VersionModDto>()
            {
                Items = [.. paginatedResponse.Items.Select(v => new VersionModDto(v))],
                Page = paginatedResponse.Page,
                PageSize = paginatedResponse.PageSize
            });
        }
    }
}
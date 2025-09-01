using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Models;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("{username}/modpacks/{slug}/versions/{iteration}/mods")]
    public class VersionModsController(PackbuilderContext context) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetMods")]
        public async Task<ActionResult<List<VersionMod>>> GetVersionMods([FromRoute] int iteration)
        {
            List<VersionMod> versionMods = await context.VersionMods.Where(v => v.VersionIteration == iteration).ToListAsync();

            if (versionMods is null)
            {
                return NotFound();
            }

            List<VersionModDto> versionModDtos = [.. versionMods.Select(v => new VersionModDto(v))];

            return Ok(versionModDtos);
        }
    }
}
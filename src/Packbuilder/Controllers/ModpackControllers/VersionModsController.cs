using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            return Ok(await context.VersionMods.Where(v => v.VersionIteration == iteration).ToListAsync());
        }
    }
}
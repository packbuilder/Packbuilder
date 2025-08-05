using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Models;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("{username}/modpacks/{slug}/{versionId}")]
    public class VersionModsController(PackbuilderContext context) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetVersionMods")]
        public async Task<ActionResult<ModpackVersion>> GetVersion([FromRoute] int versionId)
        {
            return Ok(await context.VersionMods.SingleOrDefaultAsync(v => v.Id == versionId));
        }
    }
}
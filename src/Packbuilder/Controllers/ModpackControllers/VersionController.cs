using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Models;
using Packbuilder.Services;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("{username}/modpacks/{slug}/versions")]
    public class VersionController(PackbuilderContext context, SessionService sessionService) : ControllerBase
    {
        [HttpGet("{versionId}")]
        [EndpointName("GetVersion")]
        public async Task<ActionResult<ModpackVersion>> GetVersion([FromRoute] int versionId)
        {
            return Ok(await context.Versions.Include(v => v.VersionMods).SingleOrDefaultAsync(v => v.Id == versionId));
        }

        [HttpPost]
        [EndpointName("CreateVersion")]
        public async Task<ActionResult<ModpackVersion>> CreateVersion([FromRoute] string slug)
        {
            Modpack? currentModpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Slug == slug);
            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser is null || currentModpack is null || currentModpack.UserId != currentUser.Id)
            {
                return Unauthorized();
            }

            ModpackVersion version = new ModpackVersion()
            {
                ModpackId = currentModpack.Id,
                Iteration = 0.0f,
                //If this causes a circular reference, use the partial here or omit this
                Modpack = currentModpack,
            };

            return Ok(version);
        }
    }
}
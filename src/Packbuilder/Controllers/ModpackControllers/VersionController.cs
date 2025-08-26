using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Services;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("{username}/modpacks/{slug}/versions")]
    public class VersionController(PackbuilderContext context, ISessionService sessionService) : ControllerBase
    {
        [HttpGet("{versionId}")]
        [EndpointName("GetVersion")]
        public async Task<ActionResult<ModpackVersion>> GetVersion([FromRoute] int versionId)
        {
            ModpackVersion? version = await context.Versions.Include(v => v.VersionMods).SingleOrDefaultAsync(v => v.Id == versionId);

            if (version == null)
            {
                return NotFound();
            }

            ModpackVersionDto versionDto = new(version, version.ModpackId, version.Iteration);

            return Ok(versionDto);
        }

        [HttpPost]
        [EndpointName("CreateVersion")]
        public async Task<ActionResult<ModpackVersion>> CreateVersion([FromRoute] string slug, [FromBody] List<int> modIds)
        {
            Modpack? currentModpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Slug == slug);
            User? currentUser = await sessionService.GetCurrentUser();

            List<Mod> mods = [];

            if (currentUser is null || currentModpack is null || currentModpack.UserId != currentUser.Id)
            {
                return Unauthorized();
            }

            modIds.ForEach(async id =>
            {
                Mod? mod = await context.Mods.SingleOrDefaultAsync(m => id == m.Id);

                if (mod != null)
                {
                    mods.Add(mod);
                }
            });

            if (mods.Count == 0)
            {
                return BadRequest();
            }

            ModpackVersion newVersion = currentModpack.CreateVersion(mods);

            foreach (VersionMod versionMod in newVersion.VersionMods)
            {
                context.VersionMods.Add(versionMod);
            }
            context.Versions.Add(newVersion);
            await context.SaveChangesAsync();
            return Ok(newVersion);
        }
    }
}
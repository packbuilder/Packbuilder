using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("{username}/modpacks/{slug}/versions")]
    public class VersionController(PackbuilderContext context, ISessionService sessionService, IVersionService versionService) : ControllerBase
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

            VersionDto versionDto = new(version, version.ModpackId, version.Iteration);

            return Ok(versionDto);
        }

        [HttpPost]
        [EndpointName("CreateVersion")]
        public async Task<ActionResult<ModpackVersion>> CreateVersion([FromRoute] string slug, [FromBody] CreateVersionDto createVersionDto)
        {
            Modpack? currentModpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Slug == slug);
            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser is null || currentModpack is null || currentModpack.UserId != currentUser.Id)
            {
                return Unauthorized();
            }

            if (createVersionDto.ModIds is null || createVersionDto.ModIds.Count <= 0) 
            {
                return BadRequest();
            }

            VersionDto? newVersion = await versionService.CreateVersion(currentModpack.Id, createVersionDto.ModIds);

            if (newVersion is null)
            {
                return NotFound();
            }

            return Ok(newVersion);
        }
    }
}
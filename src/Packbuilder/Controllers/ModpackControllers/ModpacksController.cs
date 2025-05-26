using System.Security.Claims;
using CurseForge.Dtos.ManifestDtos;
using CurseForge.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Attributes;
using Packbuilder.Dto;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Dto.Update;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.RateLimits;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("/modpacks")]
    public class ModpacksController(PackbuilderContext context, IModpackService modpackService, ICurseForgeManifestService curseForgeManifestService) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet("/user-modpacks/{userId:int}")]
        [EndpointName("GetUserModpacks")]
        public async Task<ActionResult<List<Modpack>?>> GetUserModpacks([FromRoute] int userId)
        {
            List<Modpack> userModpacks = await context.Modpacks
                .Include(m => m.User)
                    .Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1))
                        .ThenInclude(v => v.VersionMods)
                            .ThenInclude(v => v.Mod)
                                .Where(m => m.UserId == userId).ToListAsync();

            List<ModpackDto> modpackDtos = userModpacks.Select(modpack => new ModpackDto(modpack)).ToList();

            return Ok(modpackDtos);
        }

        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet("{modpackId}")]
        [EndpointName("GetModpack")]
        public async Task<ActionResult<ModpackDto>> GetModpack([FromRoute] int modpackId)
        {
            Modpack? modpack = await context.Modpacks
                .Include(m => m.User)
                    .Include(m => m.Versions.OrderByDescending(v => v.Iteration))
                        .ThenInclude(v => v.VersionMods)
                            .ThenInclude(v => v.Mod)
                                .SingleOrDefaultAsync(m => m.Id == modpackId);

            if (modpack is null)
            {
                return NotFound("This modpack does not exist.");
            }

            ModpackDto modpackDto = new(modpack);

            return Ok(modpackDto);
        }

        [RateLimit(RateLimitBuckets.Downloads)]
        [HttpGet("{modpackId}/download/version/{versionIteration}")]
        [EndpointName("DownloadCurseforgeManifest")]
        public async Task<ActionResult> DownloadCurseForgeManifest([FromRoute] int modpackId, [FromRoute] float versionIteration)
        {
            MemoryStream manifestZip = await modpackService.GetCurseForgeModpackManifest(modpackId, versionIteration);

            return File(manifestZip, "application/zip", "manifest.zip");
        }
        
        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPost]
        [EndpointName("CreateModpack")]
        public async Task<ActionResult> PostModpack([FromBody] CreateModpackDto body)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            if(body.Name is null || body.Name.Length == 0)
            {
                return BadRequest();
            }

            await modpackService.CreateModpackAsync(body, userId);
            
            return Created();
        }
        
        [RateLimit(RateLimitBuckets.Uploads)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPost("import")]
        [Consumes("multipart/form-data")]
        [EndpointName("ImportModpack")]
        public async Task<ActionResult> ImportCurseforgeModpack([FromForm] ImportModpackDto body)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);            

            CurseForgeManifestDto manifestDto = await curseForgeManifestService.ParseManifestFile(body.File);

            await modpackService.ImportCurseForgeModpackAsync(manifestDto, userId, body.AvatarDto);

            return Created();
        }

        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPut("{modpackId}")]
        [EndpointName("UpdateModpack")]
        public async Task<ActionResult> UpdateModpack([FromRoute] int modpackId, [FromBody] UpdateModpackDto body)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            Modpack? modpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId
            && m.UserId == userId);

            if (modpack is null)
            {
                return NotFound("Could not update modpack details because it does not exist.");
            }

            await modpackService.UpdateModpackAsync(body, modpackId);

            return NoContent();
        }

        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpDelete("{modpackId}")]
        [EndpointName("DeleteModpack")]
        public async Task<ActionResult> DeleteModpack([FromRoute]int modpackId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            Modpack? modpack = await context.Modpacks.Include(m => m.User).SingleOrDefaultAsync(m => m.Id == modpackId && m.UserId == userId);
            
            if (modpack is null)
            {
                return NotFound("This modpack does not exist.");
            }

            await modpackService.DeleteModpackAsync(modpackId);

            return NoContent();
        }
    }
}

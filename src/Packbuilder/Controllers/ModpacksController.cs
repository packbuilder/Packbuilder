using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Dto.Update;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("{username}/modpacks")]
    public class ModpacksController(PackbuilderContext context, ISessionService sessionService) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetAllModpacks")]
        public async Task<ActionResult<Modpack[]>> GetAllModpacks()
        {
            User? curUser = await sessionService.GetCurrentUser();

            if(curUser is null)
            {
                return Unauthorized();
            }

            Modpack[]? modpacks = await context.Modpacks.Where(m => m.UserId == curUser.Id)
                .Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1)).ToArrayAsync();

            if (modpacks is null)
            {
                return NotFound();
            }

            return Ok(modpacks);
        }

        [HttpGet("{slug}")]
        [EndpointName("GetModpack")]
        public async Task<ActionResult<Modpack>> GetModpack([FromRoute] string slug)
        {
            Modpack? modpack = await context.Modpacks
                .Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1))
                    .ThenInclude(v => v.VersionMods)
                        .SingleOrDefaultAsync(m => m.Slug == slug);

            if (modpack is null)
            {
                return NotFound();
            }

            ModpackDto modpackDto = new(modpack)
            {
                Name = modpack.Name,
                Slug = modpack.Slug,
                UserId = modpack.UserId
            };

            return Ok(modpackDto);
        }

        [HttpPost]
        [EndpointName("CreateModpack")]
        public async Task<ActionResult<Modpack>> PostModpack([FromBody] CreateModpackDto body)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser is null)
            {
                return Unauthorized();
            }

            Modpack modpack = new()
            {
                Name = body.Name,
                UserId = currentUser.Id,
                Slug = Modpack.GenerateSlug(currentUser.Name),
                Avatar = "Use generic avatar link or something"
            };

            ModpackDto modpackDto = new(modpack)
            {
                Name = body.Name,
                UserId = currentUser.Id,
                Slug = Modpack.GenerateSlug(currentUser.Name)
            };

            context.Modpacks.Add(modpack);
            await context.SaveChangesAsync();
            return Ok(modpackDto);
        }

        [HttpPut("{slug}")]
        [EndpointName("UpdateModpack")]
        public async Task<ActionResult<Modpack>> UpdateModpack([FromRoute] string userName, [FromRoute] string slug, [FromBody] UpdateModpackDto body)
        {
            Modpack? modpack = await context.Modpacks.Include(m => m.User).SingleOrDefaultAsync(m => m.Slug == slug
            && m.User.Name == userName);

            if (modpack is null)
            {
                return NotFound();
            }

            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser?.Id != modpack.UserId)
            {
                return Unauthorized();
            }

            modpack.Name = body.Name;
            modpack.Avatar = body.Avatar;

            ModpackDto modpackDto = new(modpack)
            {
                Name = modpack.Name,
                Slug = modpack.Slug,
                UserId = modpack.UserId
            };

            context.Modpacks.Update(modpack);
            await context.SaveChangesAsync();
            return Ok(modpackDto);
        }

        [HttpDelete("{slug}")]
        [EndpointName("DeleteModpack")]
        public async Task<ActionResult<Modpack>> DeleteModpack([FromRoute] string userName, [FromRoute]string slug)
        {
            Modpack? modpack = await context.Modpacks.Include(m => m.User).SingleOrDefaultAsync(m => m.Slug == slug
            && m.User.Name == userName);

            if (modpack is null)
            {
                return NotFound();
            }

            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser?.Id != modpack.UserId)
            {
                return Unauthorized();
            }

            ModpackDto modpackDto = new(modpack)
            {
                Name = modpack.Name,
                Slug = modpack.Slug,
                UserId = modpack.UserId
            };

            context.RemoveRange(modpack);
            await context.SaveChangesAsync();
            return Ok(modpackDto);
        }
    }
}

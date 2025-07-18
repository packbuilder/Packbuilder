using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("{username}/modpacks")]
    public class ModpacksController(PackbuilderContext context, ISessionService sessionService) : ControllerBase
    {
        [HttpGet("{slug}")]
        [EndpointName("GetModpack")]
        public async Task<ActionResult<Modpack>> GetModpack([FromRoute] string slug)
        {
            // TODO: Figure out how to get the theninclude shit working
            Modpack? modpack = await context.Modpacks
                .Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1))
                    .SingleOrDefaultAsync(m => m.Slug == slug);

            //         .ThenInclude(v => v.VersionMods)
            //             .ThenInclude(v => v.ModId).

            if (modpack is null)
            {
                return NotFound();
            }

            return Ok(modpack);
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

            Modpack modpack = new Modpack()
            {
                Name = body.Name,
                UserId = currentUser.Id,
                Slug = Modpack.GenerateSlug(currentUser.Name),
                Avatar = "Use generic avatar link or something"
            };

            context.Modpacks.Add(modpack);
            await context.SaveChangesAsync();
            return Ok(modpack);
        }

        [HttpPut("{slug}")]
        [EndpointName("UpdateModpack")]
        public async Task<ActionResult<Modpack>> UpdateModpack(string userName, string slug, [FromBody] CreateModpackDto body)
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

            context.Modpacks.Update(modpack);
            await context.SaveChangesAsync();
            return Ok(modpack);
        }

        [HttpDelete("{slug}")]
        [EndpointName("DeleteModpack")]
        public async Task<ActionResult<Modpack>> DeleteModpack(string userName, string slug)
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

            context.RemoveRange(modpack.Versions);
            await context.SaveChangesAsync();
            return Ok(modpack);
        }
    }
}

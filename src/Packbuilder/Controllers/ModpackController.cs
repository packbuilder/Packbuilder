using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("{username}/modpacks")]
    public class ModpackController(PackbuilderContext context, ISessionService sessionService) : ControllerBase
    {
        [HttpGet("{slug:string}")]
        [EndpointName("GetModpacks")]
        public async Task<ActionResult<User>> GetModpack([FromRoute] int id)
        {
            return Ok(await context.Modpacks.SingleOrDefaultAsync(m => m.Id == id));
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

        [HttpPut("{slug:string}")]
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

        [HttpDelete("{slug:string}")]
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

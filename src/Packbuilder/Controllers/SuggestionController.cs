using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("{username}/modpacks/{slug}/suggestions")]
    public class SuggestionController(PackbuilderContext context, ISessionService sessionService) : ControllerBase
    {
        [HttpGet]
        [EndpointName("GetSuggestions")]
        public async Task<ActionResult<Suggestion>> GetUser([FromRoute] string slug)
        {
            return Ok(await context.Suggestions.SingleOrDefaultAsync(s => s.ModpackSlug == slug));
        }

        [HttpPost]
        [EndpointName("PostSuggestion")]
        public async Task<ActionResult<Suggestion>> PostSuggestion([FromBody] CreateSuggestionDto body, [FromRoute] string slug)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser is null)
            {
                return Unauthorized();
            }

            Suggestion suggestion = new Suggestion()
            {
                ModpackSlug = slug,
                Username = currentUser.Name,
                Memo = body.Memo
            };

            context.Suggestions.Add(suggestion);
            await context.SaveChangesAsync();
            return Ok(suggestion);
        }

        [HttpPut]
        [Route("UpdateSuggestion")]
        public async Task<ActionResult<Suggestion>> UpdateSuggestion([FromBody] CreateSuggestionDto body, [FromRoute] string slug)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser is null)
            {
                return Unauthorized();
            }

            Suggestion? suggestion = await context.Suggestions.Include(s => s.User).Include(s => s.Modpack)
                .SingleOrDefaultAsync(s => s.ModpackSlug == slug && s.User.Name == currentUser.Name);

            if (suggestion is null)
            {
                return NotFound();
            }

            suggestion.Memo = body.Memo;

            context.Suggestions.Add(suggestion);
            await context.SaveChangesAsync();
            return Ok(suggestion);
        }

        [HttpDelete]
        [Route("DeleteSuggestion")]
        public async Task<ActionResult<Suggestion>> DeleteSuggestion([FromRoute] string slug)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser is null)
            {
                return Unauthorized();
            }

            Suggestion? suggestion = await context.Suggestions.Include(s => s.User).Include(s => s.Modpack)
                .SingleOrDefaultAsync(s => s.ModpackSlug == slug && s.User.Name == currentUser.Name);

            if (suggestion is null)
            {
                return NotFound();
            }

            context.Suggestions.Remove(suggestion);
            await context.SaveChangesAsync();
            return Ok(suggestion);
        }
    }
}
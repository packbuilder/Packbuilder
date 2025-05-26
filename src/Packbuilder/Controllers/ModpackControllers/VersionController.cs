using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Attributes;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Jobs;
using Packbuilder.Models;
using Packbuilder.Models.enums;
using Packbuilder.RateLimits;
using StackExchange.Redis;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("/modpacks/{modpackId}/versions")]
    public class VersionController(PackbuilderContext context, ISuggestionService suggestionService, IConnectionMultiplexer redis) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet("{versionId}")]
        [EndpointName("GetVersion")]
        public async Task<ActionResult<ModpackVersion>> GetVersion([FromRoute] int versionId)
        {
            ModpackVersion? version = await context.Versions.Include(v => v.VersionMods).ThenInclude(v => v.Mod).SingleOrDefaultAsync(v => v.Id == versionId);

            if (version == null)
            {
                return NotFound();
            }

            VersionDto versionDto = new(version);

            return Ok(versionDto);
        }
        
        [RateLimit(RateLimitBuckets.Jobs)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPost("create/{suggestionId}")]
        [EndpointName("MergeSuggestion")]
        public async Task<ActionResult> MergeSuggestion([FromRoute] int modpackId, [FromRoute] int suggestionId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            Modpack? currentModpack = await context.Modpacks.Include(m => m.User).SingleOrDefaultAsync(m => m.Id == modpackId);

            Suggestion? currentSuggestion = await context.Suggestions.Include(s => s.Modifications).SingleOrDefaultAsync(s => s.Id == suggestionId);

            if(currentModpack is null || currentSuggestion is null)
            {
                return NotFound("Could not merge suggestion into modpack because one or both don't exist.");
            }

            if (currentModpack.User.Id != userId)
            {
                return Unauthorized("You are not allowed to merge a suggestion into this modpack.");
            }

            if(currentSuggestion.State != SuggestionState.Verified || currentSuggestion.Modifications.Count == 0)
            {
                return BadRequest("This suggestion cannot be merged in it's current state.");
            }

            await suggestionService.UpdateSuggestionState(suggestionId, SuggestionState.MergePending);

            await redis.GetSubscriber().PublishAsync(MergeSuggestionJob.ChannelName, suggestionId);

            return NoContent();
        }
    }
}
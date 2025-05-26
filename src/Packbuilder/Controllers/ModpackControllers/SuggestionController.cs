using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Packbuilder.Attributes;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Events;
using Packbuilder.Interfaces;
using Packbuilder.Jobs;
using Packbuilder.Models;
using Packbuilder.Models.enums;
using Packbuilder.RateLimits;
using StackExchange.Redis;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("modpacks/{modpackId}/suggestions")]
    public class SuggestionController(PackbuilderContext context, ISuggestionService suggestionService, IConnectionMultiplexer redis, IEventDispatcher eventDispatcher) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet("{id:int}")]
        [EndpointName("GetSuggestion")]
        public async Task<ActionResult<SuggestionDto>> GetSuggestion([FromRoute] int modpackId, [FromRoute] int id)
        {
            Suggestion? suggestion = await context.Suggestions.Include(s => s.User).Include(s => s.Modifications).ThenInclude(m => m.Mod).SingleOrDefaultAsync(s => s.ModpackId == modpackId && s.Id == id);

            if (suggestion is null)
            {
                return NotFound();
            }

            SuggestionDto suggestionDto = new(suggestion);

            return Ok(suggestionDto);
        }

        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet]
        [EndpointName("GetModpackSuggestions")]
        public async Task<ActionResult<List<SuggestionDto>>> GetModpackSuggestions([FromRoute] int modpackId)
        {
            List<Suggestion>? suggestions = await context.Suggestions.Include(s => s.User).Include(s => s.Modifications).ThenInclude(m => m.Mod).Where(s => s.ModpackId == modpackId).ToListAsync();

            if (suggestions is null)
            {
                return NotFound();
            }

            List<SuggestionDto> suggestionDtos = suggestions.Select(suggestion => new SuggestionDto(suggestion)).ToList();
            
            return Ok(suggestionDtos);
        }

        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet("/user/{userId:int}/suggestions")]
        [EndpointName("GetUserSuggestions")]
        public async Task<ActionResult<List<SuggestionDto>>> GetUserSuggestions([FromRoute] int userId)
        {
            List<Suggestion>? suggestions = await context.Suggestions.Include(s => s.User).Include(s => s.Modpack).Include(s => s.Modifications).ThenInclude(m => m.Mod).Where(s => s.UserId == userId).ToListAsync();

            if (suggestions is null)
            {
                return NotFound();
            }

            List<SuggestionDto> suggestionDtos = suggestions.Select(suggestion => new SuggestionDto(suggestion)).ToList();
            
            return Ok(suggestionDtos);
        }

        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPost]
        [EndpointName("PostSuggestion")]
        public async Task<ActionResult> PostSuggestion([FromBody] CreateSuggestionDto body, [FromRoute] int modpackId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            int createdSuggestionId = await suggestionService.CreateSuggestionAsync(userId, body, modpackId);

            await eventDispatcher.PublishAsync<ModpackUpdatedEvent>(new(modpackId));
            
            return Ok(createdSuggestionId);
        }

        [RateLimit(RateLimitBuckets.Jobs)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPost("{suggestionId:int}/verify")]
        [EndpointName("VerifySuggestion")]
        public async Task<ActionResult> VerifySuggestion([FromRoute] int suggestionId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            Suggestion? currentSuggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId && s.UserId == userId);

            if (currentSuggestion is null)
            {
                return NotFound("Could not verify suggestion because it doesn't exist.");
            }

            if(currentSuggestion.State != SuggestionState.Unverified)
            {
                return BadRequest("Could not verify suggestion because it is not unverified.");
            }
            
            await suggestionService.UpdateSuggestionState(suggestionId, SuggestionState.VerificationPending);

            await redis.GetSubscriber().PublishAsync(VerificationJob.ChannelName, suggestionId);

            return NoContent();
        }

        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPut("{suggestionId:int}")]
        [EndpointName("UpdateSuggestion")]
        public async Task<ActionResult> UpdateSuggestion([FromRoute] int suggestionId, [FromBody] CreateSuggestionDto body)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId && s.UserId == userId);

            if (suggestion is null)
            {
                return NotFound("Could not update suggestion because it doesn't exist.");
            }

            await suggestionService.UpdateSuggestionAsync(body, suggestionId);

            await eventDispatcher.PublishAsync<SuggestionUpdatedEvent>(new(suggestion.ModpackId, suggestionId));

            return NoContent();
        }

        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpDelete("{suggestionId:int}")]
        [EndpointName("DeleteSuggestion")]
        public async Task<ActionResult<SuggestionDto>> DeleteSuggestion([FromRoute] int suggestionId)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId && s.UserId == userId);

            if (suggestion is null)
            {
                return NotFound("Could not delete suggestion because it doesn't exist.");
            }

            await suggestionService.DeleteSuggestionAsync(suggestionId);

            await eventDispatcher.PublishAsync<ModpackUpdatedEvent>(new(suggestion.ModpackId));

            return NoContent();
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Packbuilder.Attributes;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Events;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;
using Packbuilder.RateLimits;

namespace Packbuilder.Controllers.ModpackControllers
{
    [ApiController]
    [Route("modpacks/{modpackId}/suggestions/{suggestionId}/modifications")]
    public class ModificationsController(PackbuilderContext context, ISessionService sessionService, IModificationService modificationService, IEventDispatcher eventDispatcher) : ControllerBase
    {
        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet]
        [EndpointName("GetAllModifications")]
        public async Task<ActionResult<List<ModificationDto>>> GetModifications([FromRoute] int suggestionId)
        {
            Modification[]? modifications = await context.Modifications.Where(m => m.SuggestionId == suggestionId).Include(m => m.Mod).ToArrayAsync();

            if (modifications is null)
            {
                return NotFound("There are currently no modifications for this modpack suggestion.");
            }

            List<ModificationDto> modificationDtos = modifications.Select(m => new ModificationDto(m)).ToList();

            return Ok(modificationDtos);
        }

        [RateLimit(RateLimitBuckets.ModpackRead)]
        [HttpGet("{id:int}")]
        [EndpointName("GetModification")]
        public async Task<ActionResult<ModificationDto>> GetModification([FromRoute] int id)
        {
            Modification? modification = await context.Modifications.Include(m => m.Mod).SingleOrDefaultAsync(s => s.Id == id);

            if (modification is null)
            {
                return NotFound("Modification not found.");
            }

            ModificationDto modificationDto = new(modification);

            return Ok(modificationDto);
        }

        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPost]
        [EndpointName("CreateModification")]
        public async Task<ActionResult<ModificationDto>> CreateModification([FromBody] CreateModificationDto body, [FromRoute] int suggestionId)
        {
            User? currentUser = await sessionService.GetCurrentUser();
    
            if(currentUser is null)
            {
                return Unauthorized("You must log in to create a modification.");
            }

            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId);

            if(suggestion is null || suggestion.UserId != currentUser.Id || suggestion.State == SuggestionState.MergePending || suggestion.State == SuggestionState.VerificationPending)
            {
                return BadRequest("Unable to create modification.");
            }

            await modificationService.CreateModificationAsync(body, suggestionId);

            await eventDispatcher.PublishAsync<SuggestionUpdatedEvent>(new(suggestion.ModpackId, suggestionId));

            return Created();
        }
        
        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpPost("bulk")]
        [EndpointName("CreateModifications")]
        public async Task<ActionResult> PostModifications([FromBody] List<CreateModificationDto> body, [FromRoute] int suggestionId)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser is null)
            {
                return Unauthorized("You must login to create modifications.");
            }

            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId);

            if(suggestion is null || suggestion.UserId != currentUser.Id || suggestion.State == SuggestionState.MergePending || suggestion.State == SuggestionState.VerificationPending)
            {
                return BadRequest("Unable to create modifications.");
            }
            
            await modificationService.CreateModificationsAsync(body, suggestionId);

            await eventDispatcher.PublishAsync<SuggestionUpdatedEvent>(new(suggestion.ModpackId, suggestionId));
            
            return Created();
        }

        [RateLimit(RateLimitBuckets.ModpackWrite)]
        [Authorize(Policy = "VerifiedEmail")]
        [HttpDelete("{modificationId:int}")]
        [EndpointName("DeleteModification")]
        public async Task<ActionResult> DeleteModification([FromRoute] int modificationId, [FromRoute] int suggestionId)
        {
            User? currentUser = await sessionService.GetCurrentUser();

            if (currentUser is null)
            {
                return Unauthorized("You must login to delete this modification.");
            }

            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId);

            if(suggestion is null || suggestion.UserId != currentUser.Id || suggestion.State == SuggestionState.MergePending || suggestion.State == SuggestionState.VerificationPending)
            {
                return BadRequest("Unable to delete this modification.");
            }

            await modificationService.DeleteModificationAsync(modificationId, suggestionId);

            await eventDispatcher.PublishAsync<SuggestionUpdatedEvent>(new(suggestion.ModpackId, suggestionId));
            
            return NoContent();
        }
    }
}
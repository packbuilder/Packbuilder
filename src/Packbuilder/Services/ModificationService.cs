using CurseForge.Dtos.CurseForgeApiDtos;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class ModificationService(PackbuilderContext context, IExternalModService externalModService, IModService modService) : IModificationService
    {
        public async Task CreateModificationAsync(CreateModificationDto dto, int suggestionId)
        {
            Suggestion? suggestion = await context.Suggestions
                .Include(s => s.Modifications)
                    .ThenInclude(m => m.Mod).SingleOrDefaultAsync(s => s.Id == suggestionId)
                        ?? throw new Exception($"Modification could not be created because suggestion with id {suggestionId} doesn't exist");

            ExternalModSummary externalModSummary = await externalModService.GetExternalModAsync(dto.ModPlatform, dto.ModReferenceId);

            Mod mod = await modService.GetOrCreateModAsync(externalModSummary);

            Modification? existingModification = suggestion.Modifications.FirstOrDefault(m => m.Mod.ReferenceId == mod.ReferenceId);

            if(existingModification is not null)
            {
                throw new Exception($"Could not add modification for mod {mod.ReferenceId} because there is already a modification for it.");
            }

            Modification modification = suggestion.CreateModification(mod, dto.ModAction);
            suggestion.State = SuggestionState.Unverified;

            context.Suggestions.Update(suggestion);
            context.Modifications.Add(modification);

            await context.SaveChangesAsync();

            return;
        }

        public async Task CreateModificationsAsync(List<CreateModificationDto> dtos, int suggestionId)
        {
            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId)  
                ?? throw new Exception($"Modification could not be created because suggestion with id {suggestionId} doesn't exist");

            List<string> modReferenceIds = dtos.Select(m => m.ModReferenceId).ToList();

            ModPlatform modPlatform = dtos[0].ModPlatform;
            
            List<ExternalModSummary> externalModSummaries = await externalModService.GetExternalModsAsync(modPlatform, modReferenceIds);

            Dictionary<string, ExternalModSummary> modsByReferenceId = externalModSummaries.ToDictionary(m => m.ReferenceId);

            foreach (CreateModificationDto dto in dtos)
            {
                ExternalModSummary modSummary = modsByReferenceId[dto.ModReferenceId];

                Mod mod = await modService.GetOrCreateModAsync(modSummary);
               
                Modification modification = suggestion.CreateModification(mod, dto.ModAction);

                context.Modifications.Add(modification);
            }
            
            suggestion.State = SuggestionState.Unverified;

            context.Suggestions.Update(suggestion);
            await context.SaveChangesAsync();

            return;
        }

        public async Task DeleteModificationAsync(int modificationId, int suggestionId)
        {
            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId)
                ?? throw new Exception($"Modification could not be created because suggestion with id {suggestionId} doesn't exist");
            Modification? modification = await context.Modifications.Include(m => m.Mod)
                .SingleOrDefaultAsync(m => m.Id == modificationId)
                    ?? throw new Exception($"Modification with id {modificationId} not found. Could not delete.");
            
            suggestion.State = SuggestionState.Unverified;

            context.Modifications.Remove(modification);
            context.Suggestions.Update(suggestion);
            await context.SaveChangesAsync();
        }
    }
}
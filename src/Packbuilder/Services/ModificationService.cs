using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class ModificationService(PackbuilderContext context) : IModificationService
    {
        public async Task CreateModificationAsync(CreateModificationDto dto, int suggestionId)
        {
            Suggestion? suggestion = await context.Suggestions
                .Include(s => s.Modifications)
                    .ThenInclude(m => m.Mod).SingleOrDefaultAsync(s => s.Id == suggestionId)
                        ?? throw new Exception($"Modification could not be created because suggestion with id {suggestionId} doesn't exist");
            Mod? mod = await context.Mods.SingleOrDefaultAsync(m => m.ReferenceId == dto.ModReferenceId);

            if(mod is null)
            {
                mod = new()
                {
                    Platform = dto.ModPlatform,
                    ReferenceId = dto.ModReferenceId
                };

                context.Mods.Add(mod);
            }

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

            foreach (CreateModificationDto dto in dtos)
            {
                Mod? mod = await context.Mods.SingleOrDefaultAsync(m => m.ReferenceId == dto.ModReferenceId);

                if(mod is null)
                {
                    mod = new()
                    {
                        Platform = dto.ModPlatform,
                        ReferenceId = dto.ModReferenceId
                    };

                    context.Mods.Add(mod);
                }

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
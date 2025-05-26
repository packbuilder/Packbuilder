using CurseForge.Dtos;
using CurseForge.enums;
using CurseForge.Interfaces;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class SuggestionService(PackbuilderContext context, ICurseForgeService curseForgeService) : ISuggestionService   
    {
        public async Task<SuggestionDto?> GetSuggestionById(int suggestionId)
        {
            Suggestion? suggestion = await context.Suggestions.Include(s => s.User).SingleOrDefaultAsync(s => s.Id == suggestionId) ?? throw new Exception($"Suggestion with id {suggestionId} not found");

            return new(suggestion);
        }
        
        public async Task<int> CreateSuggestionAsync(int userId, CreateSuggestionDto body, int modpackId)
        {
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId) ?? throw new Exception("Could not create suggestion because user does not exist");

            Modpack? currentModpack = await context.Modpacks.
                Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1))
                    .SingleOrDefaultAsync(m => m.Id == modpackId);
            
            Suggestion? existingSuggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.UserId == userId && s.ModpackId == modpackId);
            
            if(currentModpack is null || existingSuggestion is not null)
            {
                throw new Exception("Could not create a new suggestion because the modpack it was created for doesn't exist or because this user already has a suggestion for this modpack.");
            }

            ModpackVersion latestVersion = currentModpack.Versions.First();

            Suggestion suggestion = Suggestion.CreateSuggestion(currentModpack, user, latestVersion, body); 
            
            context.Suggestions.Add(suggestion);
            await context.SaveChangesAsync();

            return suggestion.Id;
        }
        
        public async Task UpdateSuggestionAsync(CreateSuggestionDto body, int suggestionId)
        {
            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId)
                ?? throw new Exception($"Could not update suggestion with id {suggestionId} because it doesn't exist");

            suggestion.Memo = body.Memo;

            if(body.GameVersion != suggestion.GameVersion || body.ModLoader != suggestion.ModLoader)
            {
                suggestion.State = SuggestionState.Unverified;
            }

            suggestion.GameVersion = body.GameVersion;
            suggestion.ModLoader = body.ModLoader;
            
            context.Suggestions.Update(suggestion);
            await context.SaveChangesAsync();
            
            return;
        }

        public async Task DeleteSuggestionAsync(int suggestionId)
        {   
            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId)
                ?? throw new Exception($"Could not delete suggestion with id {suggestionId} because it doesn't exist");

            context.RemoveRange(suggestion);
            await context.SaveChangesAsync();

            return;
        }

        public async Task OutdateSuggestions(int modpackId, ModpackVersion latestVersion)
        {
            Modpack? modpack = await context.Modpacks.SingleOrDefaultAsync(m => m.Id == modpackId);

            if (modpack is null)
            {
                return;
            }

            List<Suggestion>? suggestions = await context.Suggestions.Where(s => s.ModpackId == modpack.Id).ToListAsync();

            if (suggestions is null || suggestions.Count == 0)
            {
                return;
            }

            foreach (Suggestion suggestion in suggestions)
            {
                if(suggestion.VersionId != latestVersion.Id || suggestion.VersionIteration != latestVersion.Iteration)
                {
                    suggestion.State = SuggestionState.Unverified;
                    context.Suggestions.Update(suggestion);
                }
            }

            await context.SaveChangesAsync();

            return;
        }

        public async Task MarkConflictsAsync(List<Modification> conflictingModifications)
        {   
            foreach (Modification modification in conflictingModifications)
            {
                modification.ConflictState = ConflictState.Conflicting;
                context.Update(modification);
            }

            await context.SaveChangesAsync();

            return;
        }

        public async Task UpdateSuggestionState(int suggestionId, SuggestionState newState)
        {
            Suggestion? suggestion = await context.Suggestions.SingleOrDefaultAsync(s => s.Id == suggestionId)
                ?? throw new Exception("Suggestion state for suggestion with id:{suggestionId} couldn't be updated because it doesn't exist.");

            suggestion.State = newState;
            context.Suggestions.Update(suggestion);
            await context.SaveChangesAsync();

            return;
        }

        public async Task<int> VerifyMinecraftSuggestionAsync(int suggestionId) {
            Suggestion? suggestion = await context.Suggestions
                .Include(s => s.Modpack)
                    .ThenInclude(m => m.Versions
                        .OrderByDescending(v => v.Iteration)
                        .Take(1))
                        .ThenInclude(v => v.VersionMods)
                            .ThenInclude(vm => vm.Mod)
                .SingleOrDefaultAsync(s => s.Id == suggestionId);
            
            if(suggestion is null || suggestion.Modpack is null || suggestion.Modpack.Versions is null || suggestion.Modpack.Versions.Count == 0)
            {
                throw new Exception($"There is no suggestion with id:{suggestionId} or the modpack tied to this suggestion does not exist or is missing data.");
            }

            MinecraftModLoader minecraftModLoader = MinecraftModLoaderMapper.TryConvertToMinecraftModLoader(suggestion.ModLoader, out bool isSupported);

            if (!isSupported)
            {
                throw new Exception($"The suggestion with id {suggestionId} is not using a minecraft mod loader so it cant be verified with this function.");
            }
            
            List<Modification> modifications = await context.Modifications
                .Include(m => m.Mod).Where(m => m.SuggestionId == suggestion.Id).ToListAsync();

            ModpackVersion latestVersion = suggestion.Modpack.Versions.First();

            HashSet<string> modpackReferenceIds = latestVersion.VersionMods
                .Select(vm => vm.Mod.ReferenceId).ToHashSet();

            List<string> modificationReferenceIds = modifications
                .Select(m => m.Mod.ReferenceId).ToList();

            ModCompatibilityResultDto? modificationsCompatibilityResult = await curseForgeService
                .CheckModCompatibility(suggestion.GameVersion, minecraftModLoader, modificationReferenceIds) 
                    ?? throw new Exception("Problem with checking compatibility of modification mods.");
            
            ModCompatibilityResultDto? modpackCompatibilityResult = await curseForgeService
                .CheckModCompatibility(suggestion.GameVersion, minecraftModLoader, modpackReferenceIds.ToList())
                    ?? throw new Exception("Problem with checking compatibility of modpack mods.");;

            List<Modification> conflictingModifications = modifications.Where(m =>
                    (m.ModAction == ModAction.Removed && !modpackReferenceIds.Contains(m.Mod.ReferenceId)) ||
                    (m.ModAction == ModAction.Added && modpackReferenceIds.Contains(m.Mod.ReferenceId))
                ).ToList();

            await ResolveMissingDependenciesAsync(suggestion, minecraftModLoader, modificationsCompatibilityResult.CompatibleMods.ToList(), modpackReferenceIds);
            
            await ResolveIncompatibleModificationsAsync(suggestion, modificationsCompatibilityResult.InCompatibleMods.ToList());
            
            await ResolveIncompatibleModpackModsAsync(suggestion, modpackCompatibilityResult.InCompatibleMods.ToList());

            foreach (Modification modification in conflictingModifications)
            {
                context.Modifications.Remove(modification);
            }

            suggestion.State = SuggestionState.Verified;
            suggestion.VersionId = latestVersion.Id;
            suggestion.VersionIteration = latestVersion.Iteration;
            context.Suggestions.Update(suggestion);

            Modpack latestModpackSnapshot = await context.Modpacks
                .AsNoTracking()
                .Where(m => m.Id == suggestion.ModpackId)
                .Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1))
                .FirstAsync();
            
            ModpackVersion latestVersionSnapshot = latestModpackSnapshot.Versions.First();

            if(latestVersionSnapshot.Iteration != latestVersion.Iteration)
            {
                throw new Exception($"Could not verify suggestion with id: {suggestionId} due to concurrency issue.");
            }

            await context.SaveChangesAsync();

            return suggestion.Id;
        }

        private async Task ResolveMissingDependenciesAsync(Suggestion suggestion, MinecraftModLoader minecraftModLoader, List<string> compatibleModReferenceIds, HashSet<string> modpackReferenceIds)
        {
            MissingDependenciesResultDto missingDependenciesDto = await curseForgeService
                .CheckForMissingDependencies(suggestion.GameVersion, minecraftModLoader, compatibleModReferenceIds)
                    ?? throw new Exception("Problem with checking for missing dependencies via curseforgeApi service.");
            
            HashSet<string> existingModificationReferenceIds = suggestion.Modifications.Select(m => m.Mod.ReferenceId).ToHashSet();
            HashSet<string> missingDependencies = missingDependenciesDto.MissingDependencies;
            HashSet<string> modsWithIncompatibleDependencies = missingDependenciesDto.ModsWithIncompatibleDependencies;

            if(missingDependencies.Count == 0) return;

            foreach (string dependencyId in missingDependencies)
            {
                Mod? mod = await context.Mods.SingleOrDefaultAsync(m => m.ReferenceId == dependencyId);

                if(mod is null)
                {
                    mod = new()
                    {
                        ReferenceId = dependencyId,
                        Platform = ModPlatform.CurseForge
                    };

                    context.Mods.Add(mod);
                }

                if(!modpackReferenceIds.Contains(dependencyId) && !existingModificationReferenceIds.Contains(dependencyId))
                {   
                    Modification newModification = suggestion.CreateModification(mod, ModAction.Added);
                    context.Modifications.Add(newModification);
                    continue;
                }
                
                if(modpackReferenceIds.Contains(dependencyId) && existingModificationReferenceIds.Contains(dependencyId))
                {
                    Modification existingModification = suggestion.Modifications.SingleOrDefault(m => m.Mod.ReferenceId == dependencyId)!;

                    context.Modifications.Remove(existingModification);
                }
            }

            foreach (Modification modification in suggestion.Modifications)
            {
                if (modsWithIncompatibleDependencies.Contains(modification.Mod.ReferenceId))
                {
                    modification.ConflictState = ConflictState.MissingDependencies;
                    context.Modifications.Update(modification);
                }
            }
        }

        private async Task ResolveIncompatibleModpackModsAsync(Suggestion suggestion, List<string> incompatibleModpackReferenceIds)
        {
            if(incompatibleModpackReferenceIds.Count == 0) return;
            
            HashSet<string> existingModificationReferenceIds = suggestion.Modifications.Select(m => m.Mod.ReferenceId).ToHashSet();

            foreach (string referenceId in incompatibleModpackReferenceIds)
            {
                Mod? mod = await context.Mods.SingleOrDefaultAsync(m => m.ReferenceId == referenceId);

                if(mod is null)
                {
                    mod = new()
                    {
                        ReferenceId = referenceId,
                        Platform = ModPlatform.CurseForge
                    };

                    context.Mods.Add(mod);
                }
                
                if(!existingModificationReferenceIds.Contains(referenceId))
                {    
                    Modification newModification = suggestion.CreateModification(mod, ModAction.Removed);
                    context.Modifications.Add(newModification);
                }
            }
        }

        private async Task ResolveIncompatibleModificationsAsync(Suggestion suggestion, List<string> incompatibleModificationReferenceIds)
        {
            if(incompatibleModificationReferenceIds.Count == 0) return;

            foreach (string referenceId in incompatibleModificationReferenceIds)
            {
                Modification? incompatibleModification = suggestion.Modifications.FirstOrDefault(m => m.Mod.ReferenceId == referenceId && m.ModAction == ModAction.Added);

                if(incompatibleModification is not null)
                {
                    context.Modifications.Remove(incompatibleModification);
                }
            }
        }
    }
}
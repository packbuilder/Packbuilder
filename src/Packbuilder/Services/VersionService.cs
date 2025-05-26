using System.Net;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class VersionService(PackbuilderContext context) : IVersionService
    {
        public async Task<VersionDto?> GetLatestVersion(int modpackId)
        {
            Modpack? modpack = await context.Modpacks.Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1)).ThenInclude(v => v.VersionMods).ThenInclude(v => v.Mod).SingleOrDefaultAsync(m => m.Id == modpackId);


            if (modpack is null)
            {
                return null;
            }

            ModpackVersion latestVersion = modpack.Versions.First();

            return new VersionDto(latestVersion);
        }

        public async Task<VersionDto?> FindVersion(int modpackId, float iteration)
        {
            ModpackVersion? version = await context.Versions.Include(v => v.VersionMods).ThenInclude(v => v.Mod).SingleOrDefaultAsync(v => v.ModpackId == modpackId && v.Iteration == iteration);

            if (version is null)
            {
                return null;
            }

            return new VersionDto(version);
        }

        public async Task MergeSuggestionAsync(int suggestionId)
        {
            Suggestion suggestion = await context.Suggestions.Include(s => s.Modifications)
                .ThenInclude(m => m.Mod).SingleOrDefaultAsync(s => s.Id == suggestionId)
                    ?? throw new Exception($"Unable to create version because suggestion with {suggestionId} does not exist");

            Modpack? modpack = await context.Modpacks
                .Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1))
                    .ThenInclude(v => v.VersionMods)
                        .ThenInclude(v => v.Mod).SingleOrDefaultAsync(m => m.Id == suggestion.ModpackId) 
                            ?? throw new Exception($"Unable to create version for modpack with id: {suggestion.ModpackId} because it doesn't exist.");

            List<Suggestion> modpackSuggestions = await context.Suggestions.Where(s => s.ModpackId == modpack.Id).ToListAsync();

            List<VersionMod> modpackMods = modpack.Versions.First().VersionMods.ToList();

            ModpackVersion latestVersion = modpack.Versions.First();

            if(suggestion.Modifications.Count <= 0)
            {
                throw new Exception("Could not create new modpack version because of a lack of modifications");
            }
            
            ModpackVersion newVersion = modpack.CreateVersionFromModifications(suggestion.Modifications.ToList(), modpackMods, suggestion.GameVersion, suggestion.ModLoader);

            foreach (VersionMod versionMod in newVersion.VersionMods)
            {
                context.VersionMods.Add(versionMod);
            }

            context.Versions.Add(newVersion);

            foreach (Suggestion modpackSuggestion in modpackSuggestions)
            {
                modpackSuggestion.State = SuggestionState.Unverified;
                context.Suggestions.Update(modpackSuggestion);
            }

            Modpack latestModpackSnapshot = await context.Modpacks
                .AsNoTracking()
                .Where(m => m.Id == suggestion.ModpackId)
                .Include(m => m.Versions.OrderByDescending(v => v.Iteration).Take(1))
                .FirstAsync();
            
            ModpackVersion latestVersionSnapshot = latestModpackSnapshot.Versions.First();

            if(latestVersionSnapshot.Iteration != latestVersion.Iteration)
            {
                throw new Exception($"Could not merge suggestion with id: {suggestionId} due to concurrency issue.");
            }

            await context.SaveChangesAsync();

            return;
        }
    }
}
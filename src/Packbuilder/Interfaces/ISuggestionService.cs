using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Dto.ServiceDtos;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Interfaces
{
    public interface ISuggestionService
    {
        public Task<SuggestionDto?> GetSuggestionById(int suggestionId);
        public Task<int> CreateSuggestionAsync(int userId, CreateSuggestionDto body,  int modpackId);
        public Task UpdateSuggestionAsync(CreateSuggestionDto body, int suggestionId);
        public Task DeleteSuggestionAsync(int suggestionId);
        public Task OutdateSuggestions(int modpackId, ModpackVersion latestVersion);
        public Task<int> VerifyMinecraftSuggestionAsync(int suggestionId);
        public Task MarkConflictsAsync(List<Modification> conflictingModifications);
        public Task UpdateSuggestionState(int suggestionId, SuggestionState newState);
    } 
}
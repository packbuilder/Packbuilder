using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Interfaces
{
    public interface IVersionService
    {
        public Task<VersionDto?> GetLatestVersion(int modpackId);
        public Task<VersionDto?> FindVersion(int modpackId, float iteration);
        public Task MergeSuggestionAsync(int suggestionId);
    } 
}
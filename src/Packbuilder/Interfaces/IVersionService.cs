using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Models;

namespace Packbuilder.Interfaces
{
    public interface IVersionService
    {
        public Task<VersionDto?> GetLatestVersion(int modpackId);
        public Task<VersionDto?> FindVersion(int modpackId, float iteration);
        public Task<VersionDto?> CreateVersion(int modpackId, List<int> modIds);
    } 
}
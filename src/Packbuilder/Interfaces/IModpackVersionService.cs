using Packbuilder.Dto.ModpackDtos;

namespace Packbuilder.Interfaces
{
    public interface IModpackVersionService
    {
        public Task<ModpackVersionDto?> GetCurrentVersion(int modpackId);
        public Task<ModpackVersionDto?> FindVersion(int modpackId, float iteration);
        public Task<ModpackVersionDto?> UpdateModpackVersion();
    } 
}
using Packbuilder.Dto.Create;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Models;

namespace Packbuilder.Interfaces
{
    public interface IVersionModService
    {
        public Task<VersionMod> CreateVersionMod(CreateVersionModDto createVersionModDto);
        public Task<VersionModDto?> FindVersionMod(float versionIteration, int modId, int modpackId);
    }
}
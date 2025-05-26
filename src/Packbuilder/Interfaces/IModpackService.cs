using CurseForge.Dtos.ManifestDtos;
using Packbuilder.Dto;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Models;

namespace Packbuilder.Interfaces
{
    public interface IModpackService
    {
        public Task CreateModpackAsync(CreateModpackDto body, int userId);
        public Task ImportCurseForgeModpackAsync(CurseForgeManifestDto manifestDto, int userId, AvatarDto body);
        public Task<MemoryStream> GetCurseForgeModpackManifest(int modpackId, float versionIteration);
        public Task UpdateModpackAsync(UpdateModpackDto body, int modpackId);
        public Task DeleteModpackAsync(int modpackId);
    }
}
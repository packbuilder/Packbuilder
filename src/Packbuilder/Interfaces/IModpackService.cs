using CurseForge.Dtos.CurseForgeApiDtos;
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
        public Task UpdateModpackAsync(UpdateModpackDto body, int modpackId);
        public Task DeleteModpackAsync(int modpackId);
    }
}
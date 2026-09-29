using CurseForge.Dtos.CurseForgeApiDtos;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Interfaces
{
    public interface IExternalModService
    {
        public Task<ExternalModSummary> GetExternalModAsync(ModPlatform modPlatform, string referenceId);
        public Task<List<ExternalModSummary>> GetExternalModsAsync(ModPlatform modPlatform, List<string> referenceIds);
    }
}
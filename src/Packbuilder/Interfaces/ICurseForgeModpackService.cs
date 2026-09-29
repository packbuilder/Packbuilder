using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.Dtos.ManifestDtos;
using Packbuilder.Dto;

namespace Packbuilder.Interfaces
{
    public interface ICurseForgeModpackService
    {
        public Task ImportCurseForgeModpackAsync(CurseForgeManifestDto manifestDto, List<ExternalModSummary> modSummaries , int userId, AvatarDto body);

        public Task<MemoryStream> GetCurseForgeModpackManifest(int modpackId, float versionIteration);
    }
}
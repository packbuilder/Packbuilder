using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.Dtos.ManifestDtos;
using Packbuilder.Dto;

namespace Packbuilder.Interfaces
{
    public interface ICurseForgeModpackService
    {
        public Task ImportMinecraftModpackAsync(CurseForgeManifestDto manifestDto, List<ExternalModSummary> modSummaries , int userId, AvatarDto body);

        public Task<MemoryStream> CreateMinecraftModpackManifest(int modpackId, float versionIteration);
    }
}
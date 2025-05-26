using CurseForge.Dtos.ManifestDtos;
using CurseForge.enums;
using Microsoft.AspNetCore.Http;

namespace CurseForge.Interfaces
{
    public interface ICurseForgeManifestService
    {
        public Task<CurseForgeManifestDto> ParseManifestFile(IFormFile file);
        public Task<MemoryStream> CreateManifestZipFile(List<string> modReferenceIds, string minecraftVersion, MinecraftModLoader minecraftModLoader, string modpackName, string modpackVersion, string ownerUsername);
    }
}
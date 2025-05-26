using System.Diagnostics.Contracts;
using CurseForge.Dtos;
using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.enums;

namespace CurseForge.Interfaces
{
    public interface ICurseForgeService
    {
        public Task<CurseForgeModLoaderSummary?> GetOptimalModLoaderVersion(string gameVersion, MinecraftModLoader minecraftModLoader);
        public Task<ModCompatibilityResultDto?> CheckModCompatibility(string gameVersion, MinecraftModLoader modLoader, List<string> modReferenceIds);
        public Task<MissingDependenciesResultDto> CheckForMissingDependencies(string gameVersion, MinecraftModLoader modLoader, List<string> modReferenceIds);
    }   
}
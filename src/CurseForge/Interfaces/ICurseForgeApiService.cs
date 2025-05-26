using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.enums;

namespace CurseForge.Interfaces
{
    public interface ICurseForgeApiService
    {   
        public Task<List<string>?> GetMinecraftVersions();
        public Task<List<CurseForgeModLoaderSummary>?> GetMinecraftModLoaders(string minecraftVersion);
        public Task<CurseForgeModList?> SearchModsAsync(int gameId, int index, int pageSize, string searchQuery, string gameVersion, MinecraftModLoader modLoader, ModSortField sortField);
        public Task<CurseForgeMod?> GetModAsync(string referenceId);
        public Task<List<CurseForgeMod>?> GetModsAsync(List<string> referenceIds);
        public Task<CurseForgeFile?> GetLatestFileAsync(string referenceId, string gameVersion, MinecraftModLoader? minecraftModLoader = null);
        public Task<List<CurseForgeCategory>?> GetMinecraftCategories();
    } 
}
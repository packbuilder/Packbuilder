using CurseForge.Dtos.CurseForgeApiDtos;

namespace CurseForge.Interfaces
{
    public interface ICurseForgeCacheService
    {
        public Task<CurseForgeFile?> GetFileDataAsync(string requestUri); 
        public Task SetFileDataAsync(CurseForgeFile curseForgeModFile, string requestUri);
        public Task SetMinecraftVersionsAsync(List<string> minecraftVersions);
        public Task<List<string>?> GetMinecraftVersionsAsync(); 
        public Task<List<CurseForgeCategory>?> GetMinecraftCategoriesAsync();
        public Task SetMinecraftCategoriesAsync(List<CurseForgeCategory> categories);
    }
}
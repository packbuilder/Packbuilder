using CurseForge.Dtos.CurseForgeApiDtos;

namespace Packbuilder.Interfaces
{
    public interface ICurseForgeModService
    {
        public Task<CurseForgeMod> GetModWithCacheAsync(string referenceId);
        public Task<List<CurseForgeMod>> GetModsWithCacheAsync(List<string> referenceIds);
    }
}
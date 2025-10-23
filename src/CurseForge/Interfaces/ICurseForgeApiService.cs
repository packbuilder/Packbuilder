using CurseForge.Models;

namespace CurseForge.Interfaces;

public interface ICurseForgeApiService
{
    public Task<CurseForgeModList?> SearchModsAsync(string searchQuery, int gameId);
    public Task<CurseForgeMod?> GetModAsync(string referenceId);
}

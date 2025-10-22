using Packbuilder.Dto.Create;
using Packbuilder.Dto.CurseForgeDtos;
using Packbuilder.Models;

namespace Packbuilder.Interfaces
{
    public interface ICurseForgeApiService
    {
        public Task<CurseForgeModListDto?> SearchModsAsync(string searchQuery, int gameId);
        public Task<CurseForgeModDto?> GetModAsync(string referenceId);
    } 
}
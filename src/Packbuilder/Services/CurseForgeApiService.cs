using Packbuilder.Dto.CurseForgeDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models.CurseForgeApiResponse;

namespace Packbuilder.Services
{
    public class CurseForgeApiService(HttpClient httpClient) : ICurseForgeApiService
    {
        public async Task<CurseForgeModListDto?> SearchModsAsync(string searchQuery, int gameId)
        {
            CurseForgeModListResponse? response = await httpClient
                .GetFromJsonAsync<CurseForgeModListResponse>($"/v1/mods/search?gameId={gameId}&searchFilter={searchQuery}");

            if (response is null || response.Data is null)
            {
                return null;
            }

            List<CurseForgeModDto> modDtos = response.Data.Select(item => new CurseForgeModDto {
                ReferenceId = item.Id,
                Slug = item.Slug,
                Name = item.Name,
                LogoUrl = item.Logo?.Url
            }).ToList();

            return new CurseForgeModListDto
            {
                Mods = modDtos,
                Pagination = response.Pagination
            };
        }

        public async Task<CurseForgeModDto?> GetModAsync(string referenceId)
        {
            CurseForgeModResponse? response = await httpClient
                .GetFromJsonAsync<CurseForgeModResponse>($"/v1/mods/{referenceId}");

            if (response is null || response.Data is null)
            {
                return null;
            }

            CurseForgeModSummary modData = response.Data;

            return new CurseForgeModDto {
                ReferenceId = modData.Id,
                Slug = modData.Slug,
                Name = modData.Name,
                LogoUrl = modData.Logo?.Url
            };
        }
    }
}
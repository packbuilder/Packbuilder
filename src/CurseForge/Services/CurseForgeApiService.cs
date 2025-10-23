using System.Net.Http.Json;
using CurseForge.Dtos;
using CurseForge.Models;

namespace CurseForge.Services;

public class CurseForgeApiService(HttpClient httpClient)
{
    public async Task<CurseForgeModListDto?> SearchModsAsync(string searchQuery, int gameId)
    {
        CurseForgeModListResponse? response = await httpClient
            .GetFromJsonAsync<CurseForgeModListResponse>($"/v1/mods/search?gameId={gameId}&searchFilter={searchQuery}");

        if (response?.Data is null)
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

        if (response?.Data is null)
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
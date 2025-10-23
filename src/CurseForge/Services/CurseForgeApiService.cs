using System.Net.Http.Json;
using CurseForge.Models;
using CurseForge.Interfaces;
using CurseForge.Options;

namespace CurseForge.Services;

public class CurseForgeApiService : ICurseForgeApiService
{
    private readonly HttpClient _httpClient = new();

    public CurseForgeApiService(CurseForgeApiOptions options)
    {
        _httpClient.BaseAddress = new Uri(options.BaseUrl);
        _httpClient.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
        _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<CurseForgeModList?> SearchModsAsync(string searchQuery, int gameId)
    {
        CurseForgeModListResponse? response = await _httpClient
            .GetFromJsonAsync<CurseForgeModListResponse>($"/v1/mods/search?gameId={gameId}&searchFilter={searchQuery}");

        if (response?.Data is null)
        {
            return null;
        }

        List<CurseForgeMod> modDtos = response.Data.Select(item => new CurseForgeMod
        {
            ReferenceId = item.Id,
            Slug = item.Slug,
            Name = item.Name,
            LogoUrl = item.Logo?.Url
        }).ToList();

        return new CurseForgeModList
        {
            Mods = modDtos,
            Pagination = response.Pagination
        };
    }

    public async Task<CurseForgeMod?> GetModAsync(string referenceId)
    {
        CurseForgeModResponse? response = await _httpClient
            .GetFromJsonAsync<CurseForgeModResponse>($"/v1/mods/{referenceId}");

        if (response?.Data is null)
        {
            return null;
        }

        CurseForgeModSummary modData = response.Data;

        return new CurseForgeMod
        {
            ReferenceId = modData.Id,
            Slug = modData.Slug,
            Name = modData.Name,
            LogoUrl = modData.Logo?.Url
        };
    }
}

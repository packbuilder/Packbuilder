using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using CurseForge.Dtos.Create;
using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.Dtos.CurseForgeResponseDtos;
using CurseForge.enums;
using CurseForge.Interfaces;
using CurseForge.Options;

namespace CurseForge.Services
{
    public class CurseForgeApiService : ICurseForgeApiService
    {
        private readonly HttpClient _httpClient = new();
        private readonly ICurseForgeCacheService _cache;

        public CurseForgeApiService(CurseForgeApiOptions options, ICurseForgeCacheService curseForgeCache)
        {
            _cache = curseForgeCache;
            _httpClient.BaseAddress = new Uri(options.BaseUrl);
            _httpClient.DefaultRequestHeaders.Add("x-api-key", options.ApiKey);
            _httpClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));   
        }
    
        public async Task<List<string>?> GetMinecraftVersions()
        {
            List<string>? cachedData = await _cache.GetMinecraftVersionsAsync();

            if(cachedData is not null)
            {
                return cachedData;
            }
            
            MinecraftVersionResponse? res = await _httpClient
                .GetFromJsonAsync<MinecraftVersionResponse>("/v1/minecraft/version");

            if(res is null || res.Data is null)
            {
                return null;
            }

            List<string> minecraftVersions = res.Data.Select(v => v.Name).ToList();

            await _cache.SetMinecraftVersionsAsync(minecraftVersions);
            
            return minecraftVersions;
        }

        public async Task<List<CurseForgeCategory>?> GetMinecraftCategories()
        {
            List<CurseForgeCategory>? cachedData = await _cache.GetMinecraftCategoriesAsync();

            if(cachedData is not null)
            {
                return cachedData;
            }

            CurseForgeCategoryResponse? categoryResponse = await _httpClient.GetFromJsonAsync<CurseForgeCategoryResponse>($"/v1/mods/search?gameId=432");

            if (categoryResponse is null || categoryResponse.Data is null)
            {
                return null;
            } 

            List<CurseForgeCategory> categories = categoryResponse.Data.ToList();  

            await _cache.SetMinecraftCategoriesAsync(categories);

            return categories;
        }

        public async Task<CurseForgeModList?> SearchModsAsync(int gameId, int index, int pageSize, string searchQuery, string gameVersion, MinecraftModLoader modLoader, ModSortField sortField)
        {
            CurseForgeModListResponse? modListResponse = await _httpClient
                .GetFromJsonAsync<CurseForgeModListResponse>($"/v1/mods/search?gameId={gameId}&index={index}&pageSize={pageSize}&searchFilter={searchQuery}&sortField={sortField}&sortOrder=desc&gameVersion={gameVersion}&modLoaderType={modLoader}");
            
            if (modListResponse is null || modListResponse.Data is null)
            {
                return null;
            } 

            List<CurseForgeMod> modDtos = modListResponse.Data.Select(CurseForgeMod.FromSummary).ToList();   

            return new CurseForgeModList
            {
                Mods = modDtos,
                Pagination = modListResponse.Pagination
            };
        }
        
        public async Task<CurseForgeFile?> GetLatestFileAsync(string referenceId, string gameVersion, MinecraftModLoader? modLoader = null)
        {
            string requestUri = $"/v1/mods/{referenceId}/files?gameVersion={gameVersion}";

            if(modLoader is not null)
            {
                requestUri += $"&modLoaderType={modLoader}";
            }

            CurseForgeFile? cachedData = await _cache.GetFileDataAsync(requestUri);

            if(cachedData is not null)
            {
                return cachedData;
            }

            CurseForgeFilesResponse? response = await _httpClient.GetFromJsonAsync<CurseForgeFilesResponse>(requestUri);

            if(response is null || response.Data is null || response.Data.Count == 0)
            {
                return null;
            }

            CurseForgeFile latestModFile = response.Data
                .Aggregate((f1, f2) => DateTime.Parse(f1.FileDate) > DateTime.Parse(f2.FileDate) ? f1 : f2);
            
            await _cache.SetFileDataAsync(latestModFile, requestUri);

            return latestModFile;
        }

        public async Task<CurseForgeMod?> GetModAsync(string referenceId)
        {
            CurseForgeModResponse? response = await _httpClient
                .GetFromJsonAsync<CurseForgeModResponse>($"/v1/mods/{referenceId}");

            if (response is null || response.Data is null)
            {
                return null;
            }

            CurseForgeModSummary modSummary = response.Data;

            return CurseForgeMod.FromSummary(modSummary);
        }

        public async Task<List<CurseForgeMod>?> GetModsAsync(List<string> referenceIds)
        {
            CreateModListDto modListDto = new()
            {
                ModIds = referenceIds,
                FilterPcOnly = true
            };
            string jsonContent = JsonSerializer.Serialize(modListDto);
            StringContent content = new(jsonContent, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync($"/v1/mods", content);
            string json = await response.Content.ReadAsStringAsync();

            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
                return null;
            }

            CurseForgeModListResponse? res = JsonSerializer.Deserialize<CurseForgeModListResponse>(json);

            if (res is null || res.Data is null)
            {
                return null;
            }

            List<CurseForgeMod> modDtos = res.Data.Select(CurseForgeMod.FromSummary).ToList();

            return modDtos;
        }

        public async Task<List<CurseForgeModLoaderSummary>?> GetMinecraftModLoaders(string minecraftVersion)
        {
            CurseForgeModLoadersResponse? res = await _httpClient.GetFromJsonAsync<CurseForgeModLoadersResponse>($"/v1/minecraft/modloader?version={minecraftVersion}&includeAll=true");

            if(res is null || res.Data is null)
            {
                return null;
            }

            return res.Data;
        }
    }
}
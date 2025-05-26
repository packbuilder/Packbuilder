using System.Text.Json;
using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.Interfaces;
using StackExchange.Redis;

namespace CurseForge.Services
{
    public class CurseForgeCacheService(IConnectionMultiplexer redis) : ICurseForgeCacheService
    {
        private readonly IDatabase _db = redis.GetDatabase();
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        public async Task<CurseForgeFile?> GetFileDataAsync(string requestUri)
        {
            string cacheKey = $"fileData:{requestUri}";  

            RedisValue value = await _db.StringGetAsync(cacheKey);

            if(!value.HasValue)
            {   
                return null;
            } else
            {
                CurseForgeFile? serializedValue = JsonSerializer.Deserialize<CurseForgeFile>((string)value!, _jsonOptions);
                await _db.KeyExpireAsync(cacheKey, TimeSpan.FromDays(3));
                return serializedValue;
            }
        }
        public async Task SetFileDataAsync(CurseForgeFile searchData, string requestUri)
        {
            string jsonSearchData = JsonSerializer.Serialize(searchData, _jsonOptions);
            string cacheKey = $"fileData:{requestUri}";

            bool status = await _db.StringSetAsync(cacheKey, jsonSearchData, TimeSpan.FromDays(3));

            if(!status)
            {
                throw new Exception("Failed to set key-value pair for pagination data.");
            }
        } 

        public async Task<List<string>?> GetMinecraftVersionsAsync()
        {
            string cacheKey = $"CurseForgeMinecraftVersions";  

            RedisValue value = await _db.StringGetAsync(cacheKey);

            if(!value.HasValue)
            {   
                return null;
            } else
            {
                List<string>? serializedValue = JsonSerializer.Deserialize<List<string>>((string)value!, _jsonOptions);
                await _db.KeyExpireAsync(cacheKey, TimeSpan.FromDays(3));
                return serializedValue;
            }
        }

        public async Task SetMinecraftVersionsAsync(List<string> minecraftVersions)
        {
            string jsonSearchData = JsonSerializer.Serialize(minecraftVersions, _jsonOptions);
            string cacheKey = $"CurseForgeMinecraftVersions";

            bool status = await _db.StringSetAsync(cacheKey, jsonSearchData, TimeSpan.FromDays(3));

            if(!status)
            {
                throw new Exception("Failed to set key-value pair for pagination data.");
            }
        }

        public async Task<List<CurseForgeCategory>?> GetMinecraftCategoriesAsync()
        {
            string cacheKey = $"CurseForgeMinecraftCategories";  

            RedisValue value = await _db.StringGetAsync(cacheKey);

            if(!value.HasValue)
            {   
                return null;
            } else
            {
                List<CurseForgeCategory>? serializedValue = JsonSerializer.Deserialize<List<CurseForgeCategory>>((string)value!, _jsonOptions);
                await _db.KeyExpireAsync(cacheKey, TimeSpan.FromDays(3));
                return serializedValue;
            }
        }

        public async Task SetMinecraftCategoriesAsync(List<CurseForgeCategory> categories)
        {
            string jsonSearchData = JsonSerializer.Serialize(categories, _jsonOptions);
            string cacheKey = $"CurseForgeMinecraftCategories";

            bool status = await _db.StringSetAsync(cacheKey, jsonSearchData, TimeSpan.FromDays(3));

            if(!status)
            {
                throw new Exception("Failed to set key-value pair for pagination data.");
            }
        }
    }
}
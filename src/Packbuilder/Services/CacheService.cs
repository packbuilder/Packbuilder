using System.Text.Json;
using Packbuilder.Dto.ServiceDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models.enums;
using StackExchange.Redis;

namespace Packbuilder.Services
{
    public class CacheService(IConnectionMultiplexer redis, ITokenService tokenService) : ICacheService
    {
        private readonly IDatabase _db = redis.GetDatabase();
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public async Task<T?> GetPaginationDataAsync<T>(string searchPayload, ModPlatform modPlatform)
        {
            string cacheKey;  

            if(modPlatform == ModPlatform.Thunderstore)
            {
                cacheKey = $"pagination:thunderstore:{searchPayload}";     
            } else
            {
                cacheKey = $"pagination:curseforge:{searchPayload}";
            }

            var value = await _db.StringGetAsync(cacheKey);

            if(!value.HasValue)
            {   
                return default;
            } else
            {
                T? serializedValue = JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
                await _db.KeyExpireAsync(cacheKey, TimeSpan.FromMinutes(15));
                return serializedValue;
            }
        }

        public async Task<T?> GetModAsync<T>(string modId, ModPlatform modPlatform)
        {
            string cacheKey;  

            if(modPlatform == ModPlatform.Thunderstore)
            {
                cacheKey = $"mod:thunderstore:{modId}";     
            } else
            {
                cacheKey = $"mod:curseforge:{modId}";
            }

            RedisValue value = await _db.StringGetAsync(cacheKey);

            if(!value.HasValue)
            {   
                return default;
            } else
            {
                T? serializedValue = JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
                await _db.KeyExpireAsync(cacheKey, TimeSpan.FromDays(3));
                return serializedValue;
            }
        }

        public async Task<ModCacheDto<T>> GetModsAsync<T>(List<string> modIds, ModPlatform modPlatform)
        {  
            List<T> cachedMods = [];

            List<string> uncachedModIds = [];

            foreach (string modId in modIds)
            {
                string cacheKey;  

                if(modPlatform == ModPlatform.Thunderstore)
                {
                    cacheKey = $"mod:thunderstore:{modId}";     
                } else
                {
                    cacheKey = $"mod:curseforge:{modId}";
                }

                RedisValue value = await _db.StringGetAsync(cacheKey);

                if(!value.HasValue)
                {   
                    uncachedModIds.Add(modId);
                } else
                {
                    T? serializedValue = JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
                    await _db.KeyExpireAsync(cacheKey, TimeSpan.FromDays(3));
                    cachedMods.Add(serializedValue!);   
                }
            }
            
            return new ModCacheDto<T>
            {
                CachedMods = cachedMods,
                UncachedModIds = uncachedModIds
            };
        }

        public async Task SetModsAsync<T>(List<RedisModDataDto<T>> modDataDtos, ModPlatform modPlatform)
        {
            foreach (RedisModDataDto<T> modDataDto in modDataDtos)
            {
                string jsonModData = JsonSerializer.Serialize(modDataDto.ModData, _jsonOptions);
                string modId = modDataDto.ModId; 
                string cacheKey;  

                if(modPlatform == ModPlatform.Thunderstore)
                {
                    cacheKey = $"mod:thunderstore:{modId}";     
                } else
                {
                    cacheKey = $"mod:curseforge:{modId}";
                }

                bool status = await _db.StringSetAsync(cacheKey, jsonModData, TimeSpan.FromDays(3));

                if(!status)
                {
                    throw new Exception("Failed to set key-value pair for a mod in redis cache");
                }
            }
        }

        public async Task SetPaginationDataAsync<T>(T searchData, string searchPayload, ModPlatform modPlatform)
        {
            string jsonSearchData = JsonSerializer.Serialize(searchData, _jsonOptions);
            string cacheKey;

            if(modPlatform == ModPlatform.Thunderstore)
            {
                cacheKey = $"pagination:thunderstore:{searchPayload}";     
            } else
            {
                cacheKey = $"pagination:curseforge:{searchPayload}";
            }

            bool status = await _db.StringSetAsync(cacheKey, jsonSearchData, TimeSpan.FromMinutes(15));

            if(!status)
            {
                throw new Exception("Failed to set key-value pair for pagination data.");
            }
        } 

        public async Task<string> SetPasswordResetToken(int userId)
        {
            IDatabase db = redis.GetDatabase();
            string cacheKey = $"reset_password:{userId}";
            string rawToken = tokenService.GenerateToken();
            string hashedToken = tokenService.HashSha256(rawToken);
            await db.StringSetAsync(cacheKey, hashedToken, TimeSpan.FromMinutes(15));

            return rawToken;
        }

        public async Task<string> SetEmailVerificationToken(int userId)
        {
            IDatabase db = redis.GetDatabase();
            string cacheKey = $"email_verify:{userId}";
            string rawToken = tokenService.GenerateToken();
            string hashedToken = tokenService.HashSha256(rawToken);
            await db.StringSetAsync(cacheKey, hashedToken, TimeSpan.FromHours(24));

            return rawToken;
        }

        public async Task<string?> GetEmailVerificationToken(int userId)
        {
            IDatabase db = redis.GetDatabase();
            string cacheKey = $"email_verify:{userId}";
            string? token = await db.StringGetAsync(cacheKey);

            if(token is not null)
            {
                await db.KeyDeleteAsync(cacheKey);
            }

            return token;
        }

        public async Task<string?> GetPasswordResetToken(int userId)
        {
            IDatabase db = redis.GetDatabase();
            string cacheKey = $"reset_password:{userId}";
            string? token = await db.StringGetAsync(cacheKey);

            if(token is not null)
            {
                await db.KeyDeleteAsync(cacheKey);
            }

            return token;
        }
    }
}
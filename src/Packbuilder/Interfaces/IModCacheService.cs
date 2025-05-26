using Packbuilder.Dto.ServiceDtos;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Interfaces
{
    public interface ICacheService
    {
        public Task<T?> GetPaginationDataAsync<T>(string searchPayload, ModPlatform modPlatform);
        public Task<T?> GetModAsync<T>(string modId, ModPlatform modPlatform);
        public Task<ModCacheDto<T>> GetModsAsync<T>(List<string> modIds, ModPlatform modPlatform);
        public Task SetModsAsync<T>(List<RedisModDataDto<T>> modDataDtos, ModPlatform modPlatform);
        public Task SetPaginationDataAsync<T>(T data, string key, ModPlatform modPlatform);
        public Task<string> SetEmailVerificationToken(int userId);
        public Task<string?> GetEmailVerificationToken(int userId);
        public Task<string> SetPasswordResetToken(int userId);
        public Task<string?> GetPasswordResetToken(int userId);
    }
}
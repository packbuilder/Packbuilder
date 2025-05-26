using Packbuilder.Dto;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Models;

namespace Packbuilder.Interfaces
{
    public interface IUserService
    {
        public Task<UserDto?> GetUserById(int userId);
        public Task<int> CreateAccountAsync(CreateUserDto body);
        public Task<bool> ResetUserEmail(ResetEmailDto body, int userId);
        public Task<bool> ResetUserPassword(string token, int userId, string password);
        public Task<bool> VerifyUserEmail(string verificationCode, int userId);
        public Task UpdateAccountAsync(UpdateUserDto body, int userId);
        public Task DeleteAccountAsync(int userId);
    }
}
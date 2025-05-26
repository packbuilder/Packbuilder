using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Dto;
using Packbuilder.Dto.Create;
using Packbuilder.Dto.Update;
using Packbuilder.Exceptions;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class UserService(PackbuilderContext context, IPasswordHasher<User> passwordHasher, ITokenService tokenService, ICacheService cacheService) : IUserService
    {
        public async Task<UserDto?> GetUserById(int userId)
        {
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId);

            if(user is null)
            {
                return null;
            }

            return new() {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                ImageType = user.ImageType,
                ImageValue = user.ImageValue
            };
        }

        public async Task<int> CreateAccountAsync(CreateUserDto body)
        {
            User user = new()
            {
                Name = body.Name,
                Email = body.Email,
                ImageType = body.ImageType,
                ImageValue = body.ImageValue
            };

            user.SetPassword(passwordHasher, body.Password);

            context.Users.Add(user);
            await context.SaveChangesAsync();

            return user.Id;
        }

        public async Task<bool> VerifyUserEmail(string token, int userId)
        {
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId) ?? throw new Exception($"User with id {userId} does not exist");
            string? cachedToken = await cacheService.GetEmailVerificationToken(userId) ?? throw new Exception($"User with id ${userId} does not have email verification token."); 
            string hashedToken = tokenService.HashSha256(token);

            if(hashedToken == cachedToken)
            {
                user.EmailVerified = true;
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> ResetUserEmail(ResetEmailDto body, int userId)
        {
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId) ?? throw new Exception($"User with id {userId} does not exist");

            PasswordVerificationResult result = user.VerifyPassword(passwordHasher, body.Password);

            if (result == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.SetPassword(passwordHasher, body.Password);
            } else if (result == PasswordVerificationResult.Failed)
            {
                return false;
            }   

            user.Email = body.NewEmail;
            user.EmailVerified = false;

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ResetUserPassword(string token, int userId, string newPassword)
        {
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId) ?? throw new Exception($"User with id {userId} does not exist");
            string? cachedToken = await cacheService.GetPasswordResetToken(userId) ?? throw new Exception($"User with id ${userId} does not have password reset token."); 
            string hashedToken = tokenService.HashSha256(token);

            if(hashedToken == cachedToken)
            {
                user.SetPassword(passwordHasher, newPassword);
                context.Users.Update(user);
                await context.SaveChangesAsync();
                return true;
            }

            return false;
        }
        
        public async Task UpdateAccountAsync(UpdateUserDto body, int userId)
        {
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId)
                ?? throw new Exception($"Unable to update user with id {userId} because it does not exist.");

            if(body.Name is not null && body.Name != null)
            {
                user.Name = body.Name;
            }

            if(body.ImageValue is not null && body.ImageValue != null)
            {
                user.ImageValue = body.ImageValue;
            }

            if(body.ImageType is { } imageType)
            {
                user.ImageType = imageType;    
            }

            context.Users.Update(user);
            await context.SaveChangesAsync();

            return;
        }

        public async Task DeleteAccountAsync(int userId)
        {
            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == userId)
                ?? throw new Exception($"Unable to delete user with id {userId} because it does not exist.");

            context.Users.RemoveRange(user);
            await context.SaveChangesAsync();

            return;
        }
    }
}
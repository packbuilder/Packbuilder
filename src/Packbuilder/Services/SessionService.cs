using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Packbuilder.Dto.Create;
using Packbuilder.Exceptions;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Options;

namespace Packbuilder.Services
{
    public class SessionService(JwtOptions jwtOptions, IPasswordHasher<User> passwordHasher, PackbuilderContext context, IHttpContextAccessor httpContextAccessor) : ISessionService
    {
        public async Task<string> CreateSession(CreateSessionDto sessionDto)
        {
            User? user = await context.Users.FirstOrDefaultAsync(u => u.Email == sessionDto.Email)
                ?? throw new NotFoundException<User>();

            PasswordVerificationResult res = user.VerifyPassword(passwordHasher, sessionDto.Password);

            if (res == PasswordVerificationResult.SuccessRehashNeeded)
            {
                user.SetPassword(passwordHasher, sessionDto.Password);
            } else if (res == PasswordVerificationResult.Failed)
            {
                throw new InvalidActionException("User input did not match the database stored password");
            }   

            return GenerateJwtToken(user);
        }
        public Task<User?> GetCurrentUser()
        {
            string? userIdStr = httpContextAccessor.HttpContext!.User.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdStr, out int userId))
            {
                return context.Users.SingleOrDefaultAsync(u => userId == u.Id);
            }

            return Task.FromResult<User?>(null); 
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim("Picture", user.Avatar),
                new Claim("UpdatedAt", user.UpdatedAt.ToString("o")),
                new Claim("CreatedAt", user.CreatedAt.ToString("o"))
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "https://api.packbuilder.io",
                audience: "https://packbuilder.io",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
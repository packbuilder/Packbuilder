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
    public class SessionService(
        JwtOptions jwtOptions, 
        IPasswordHasher<User> passwordHasher, 
        PackbuilderContext context, 
        IHttpContextAccessor httpContextAccessor) : ISessionService
    {
        public async Task CreateSession(CreateSessionDto sessionDto)
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

            string token = GenerateJwtToken(user);

            httpContextAccessor.HttpContext?.Response.Cookies.Append("access_token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(2)
            });

            return;
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
        public async Task<string> RefreshSession()
        {
            string? userIdStr = (httpContextAccessor.HttpContext!.User.Claims
                .SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value) 
                    ?? throw new Exception("Cannot re-issue jwt token, unable to read user claim.");

            User? user = await context.Users.SingleOrDefaultAsync(u => u.Id == int.Parse(userIdStr)) ?? throw new Exception("Cannot re-issue jwt token. User does not exist.");

            return GenerateJwtToken(user);
        }
        public async Task DeleteSession()
        {
            httpContextAccessor.HttpContext?.Response.Cookies.Delete("access_token");
        }
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("EmailVerified", user.EmailVerified.ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "https://api.packbuilder.org",
                audience: "https://packbuilder.org",
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
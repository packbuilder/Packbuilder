using System.Security.Cryptography;
using System.Text;
using Packbuilder.Interfaces;

namespace Packbuilder.Services
{
    public class TokenService : ITokenService
    {
         public string GenerateToken(int size = 32)
        {
            byte[] tokenBytes = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            return Convert.ToBase64String(tokenBytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "");
        }
        public string HashSha256(string rawData)
        {
            byte[] shaBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawData));

            StringBuilder builder = new();

            foreach (byte t in shaBytes)
            {
                builder.Append(t.ToString("x2"));
            }

            return builder.ToString();
        }
    }
}
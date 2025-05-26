namespace Packbuilder.Interfaces
{
    public interface ITokenService
    {
        public string GenerateToken(int size = 32);
        public string HashSha256(string rawData);
    }
}
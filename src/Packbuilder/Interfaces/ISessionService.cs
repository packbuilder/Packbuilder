using Packbuilder.Dto.Create;
using Packbuilder.Models;

namespace Packbuilder.Interfaces
{
    public interface ISessionService
    {
        public Task CreateSession(CreateSessionDto sessionDto);
        public Task DeleteSession();
        public Task<string> RefreshSession();
        public Task<User?> GetCurrentUser();
    } 
}
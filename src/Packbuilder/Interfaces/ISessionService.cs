using Packbuilder.Dto;
using Packbuilder.Models;

namespace Packbuilder.Interfaces
{
    public interface ISessionService
    {
        public Task<string> CreateSession(CreateSessionDto sessionDto);
        public Task<User?> GetCurrentUser();
    } 
}
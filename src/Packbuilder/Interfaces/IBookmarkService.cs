using Packbuilder.Dto.ServiceDtos;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Interfaces
{
    public interface IBookmarkService
    {
        public Task<Bookmark?> GetBookmarkAsync(int userId, int modpackId);
        public Task<List<Bookmark>?> GetAllBookmarksAsync(int userId);
        public Task CreateBookmarkAsync(int userId, int modpackId);
        public Task DeleteBookmarkAsync(int userId, int modpackId);
    }
}
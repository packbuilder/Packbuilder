using Microsoft.EntityFrameworkCore;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class BookmarkService(PackbuilderContext context) : IBookmarkService
    {
        public async Task<Bookmark?> GetBookmarkAsync(int userId, int modpackId)
        {
            Bookmark? bookmark = await context.Bookmarks.Include(b => b.Modpack).ThenInclude(m => m.User).FirstOrDefaultAsync(b => b.UserId == userId && b.ModpackId == modpackId);

            return bookmark;
        }
        public async Task<List<Bookmark>?> GetAllBookmarksAsync(int userId)
        {
            List<Bookmark>? bookmarks = await context.Bookmarks.Include(b => b.Modpack).ThenInclude(m => m.User).Where(b => b.UserId == userId).ToListAsync();

            return bookmarks;
        }

        public async Task CreateBookmarkAsync(int userId, int modpackId)
        {
            Bookmark? existingBookmark = await context.Bookmarks.SingleOrDefaultAsync(b => b.UserId == userId && b.ModpackId == modpackId);

            if(existingBookmark is not null)
            {
                throw new Exception("Current user has already bookmarked this modpack");
            }

            context.Bookmarks.Add(new()
            {
                UserId = userId,
                ModpackId = modpackId
            });
            await context.SaveChangesAsync();
            
            return;
        }

        public async Task DeleteBookmarkAsync(int userId, int modpackId)
        {
            Bookmark? bookmark = await context.Bookmarks.SingleOrDefaultAsync(b => b.UserId == userId && b.ModpackId == modpackId) 
                ?? throw new Exception("Could not delete bookmark because it doesn't exist");
            
            context.Bookmarks.Remove(bookmark);
            await context.SaveChangesAsync();
            
            return;
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Attributes;
using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("/bookmarks")]
    public class BookmarksController(PackbuilderContext context, ISessionService sessionService, IBookmarkService bookmarkService, IPaginationService paginationService) : ControllerBase
    {
        [RateLimit]
        [HttpGet("{modpackId}")]
        [EndpointName("GetBookmark")]
        public async Task<ActionResult<Bookmark?>> GetBookmark([FromRoute] int modpackId)
        {
            User? curUser = await sessionService.GetCurrentUser();

            if (curUser is null)
            {
                return Unauthorized();
            }

            Bookmark? bookmark = await bookmarkService.GetBookmarkAsync(curUser.Id, modpackId);

            return Ok(bookmark);
        }

        [RateLimit]
        [HttpGet]
        [EndpointName("GetAllBookmarks")]
        public async Task<ActionResult<List<Bookmark>>> GetAllBookmarks([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string searchQuery = "")
        {
            User? curUser = await sessionService.GetCurrentUser();

            if (curUser is null)
            {
                return Unauthorized();
            }

            IQueryable<Bookmark>? query = context.Bookmarks.Include(b => b.Modpack).ThenInclude(m => m.User).Where(b => b.UserId == curUser.Id).Where(b => b.Modpack.Name.Contains(searchQuery));

            PaginatedResponse<Bookmark> paginatedResponse = await paginationService.GetPaginatedData(query, page, pageSize);

            return Ok(new PaginatedResponse<BookmarkDto>()
            {
                Items = [..paginatedResponse.Items.Select(b => new BookmarkDto(b))],
                Page = paginatedResponse.Page,
                PageSize = paginatedResponse.PageSize
            });
        }

        [RateLimit]
        [Authorize]
        [HttpPost("{modpackId}")]
        [EndpointName("CreateBookmark")]
        public async Task<ActionResult> CreateBookmark(int modpackId)
        {
            User? curUser = await sessionService.GetCurrentUser();

            if (curUser is null)
            {
                return Unauthorized();
            }

            await bookmarkService.CreateBookmarkAsync(curUser.Id, modpackId);

            return Created();
        }

        [RateLimit]
        [Authorize]
        [HttpDelete("{modpackId}")]
        [EndpointName("DeleteBookmark")]
        public async Task<ActionResult> DeleteBookmark(int modpackId)
        {
            User? curUser = await sessionService.GetCurrentUser();

            if (curUser is null)
            {
                return Unauthorized();
            }

            await bookmarkService.DeleteBookmarkAsync(curUser.Id, modpackId);

            return NoContent();
        }
    }
}
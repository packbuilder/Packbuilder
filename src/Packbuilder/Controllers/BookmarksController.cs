using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("/bookmarks")]
    public class BookmarksController(ISessionService sessionService, IBookmarkService bookmarkService) : ControllerBase
    {
        [HttpGet("{modpackId}")]
        [EndpointName("GetBookmark")]
        public async Task<ActionResult<Bookmark?>> GetBookmark(int modpackId)
        {
            User? curUser = await sessionService.GetCurrentUser();

            if (curUser is null)
            {
                return Unauthorized();
            }

            Bookmark? bookmark = await bookmarkService.GetBookmarkAsync(curUser.Id, modpackId);

            return Ok(bookmark);
        }

        [HttpGet]
        [EndpointName("GetAllBookmarks")]
        public async Task<ActionResult<List<Bookmark>>> GetAllBookmarks()
        {
            User? curUser = await sessionService.GetCurrentUser();

            if (curUser is null)
            {
                return Unauthorized();
            }

            List<Bookmark>? userBookmarks = await bookmarkService.GetAllBookmarksAsync(curUser.Id);

            return Ok(userBookmarks ?? []);
        }

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
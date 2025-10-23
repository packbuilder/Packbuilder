using CurseForge.Services;
using CurseForge.Interfaces;
using CurseForge.Models;
using Microsoft.AspNetCore.Mvc;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("curseforge")]
    public class CurseForgeApiController(ICurseForgeApiService curseForgeApiService) : ControllerBase
    {
        [HttpGet("{referenceId}")]
        [EndpointName("GetMod")]
        public async Task<ActionResult<CurseForgeMod>> GetMod([FromRoute] int referenceId)
        {
            CurseForgeMod? data = await curseForgeApiService.GetModAsync(referenceId.ToString());

            if (data is null)
            {
                return NotFound(null);
            }

            return Ok(data);
        }

        // TODO: This endpoint should also handle pagination query params based off curseforges pagination usage/implementation in order to fetch related data.
        [HttpGet("{gameId}/{searchQuery}")]
        [EndpointName("SearchMods")]
        public async Task<ActionResult<CurseForgeModList?>> SearchMods([FromRoute] int gameId, [FromRoute] string searchQuery)
        {
            CurseForgeModList? data = await curseForgeApiService.SearchModsAsync(searchQuery, gameId);

            if (data is null)
            {
                return NotFound(null);
            }

            return Ok(data);
        }
    }

}

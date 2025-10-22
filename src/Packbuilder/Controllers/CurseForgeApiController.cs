using Microsoft.AspNetCore.Mvc;
using Packbuilder.Interfaces;
using Packbuilder.Dto.CurseForgeDtos;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("curseforge")]
    public class CurseForgeApiController(ICurseForgeApiService curseForgeApiService) : ControllerBase
    {
        [HttpGet("{referenceId}")]
        [EndpointName("GetMod")]
        public async Task<ActionResult<CurseForgeModDto>> GetMod([FromRoute] int referenceId)
        {
            CurseForgeModDto? data = await curseForgeApiService.GetModAsync(referenceId.ToString());

            if (data is null)
            {
                return NotFound(null);
            }

            return Ok(data);
        }

        // TODO: This endpoint should also handle pagination query params based off curseforges pagination usage/implementation in order to fetch related data.
        [HttpGet("{gameId}/{searchQuery}")]
        [EndpointName("SearchMods")]
        public async Task<ActionResult<CurseForgeModListDto?>> SearchMods([FromRoute] int gameId, [FromRoute] string searchQuery)
        {
            CurseForgeModListDto? data = await curseForgeApiService.SearchModsAsync(searchQuery, gameId);

            if (data is null)
            {
                return NotFound(null);
            }

            return Ok(data);
        }
    }
    
}
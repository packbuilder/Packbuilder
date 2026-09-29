using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Attributes;
using Packbuilder.Models;

namespace Packbuilder.Controllers
{
    [ApiController]
    [Route("/mods")]
    public class ModController(PackbuilderContext context) : ControllerBase
    {
        [RateLimit]
        [HttpPost]
        [EndpointName("GetMods")]
        public async Task<ActionResult<Mod[]>> GetMods([FromBody] int[] modIds)
        {
            Mod[]? mods = await context.Mods.Where(m => modIds.Contains(m.Id)).ToArrayAsync();

            if (mods is null)
            {
                return NotFound();
            }

            return Ok(mods);
        }

        [RateLimit]
        [HttpPost("/mods/referenceIds")]
        [EndpointName("GetModReferenceIds")]
        public async Task<ActionResult<Mod[]>> GetModReferenceIds([FromBody] int[] modIds)
        {
            string[]? referenceIds = await context.Mods.Where(m => modIds.Contains(m.Id)).Select(m => m.ReferenceId).ToArrayAsync();

            if (referenceIds is null)
            {
                return NotFound();
            }

            return Ok(referenceIds);
        }
    }
}
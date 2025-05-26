using Microsoft.EntityFrameworkCore;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class ModService(PackbuilderContext context) : IModService
    {
        public async Task<Mod> GetOrCreateMod(ModPlatform platform, string referenceId)
        {
            Mod? mod = await context.Mods.SingleOrDefaultAsync(m => m.ReferenceId == referenceId);

            if(mod is null)
            {    
                mod = new()
                {
                    Platform = platform,
                    ReferenceId = referenceId
                };

                context.Mods.Add(mod);
                await context.SaveChangesAsync();

                return mod;
            }

            return mod;
        }
    }
}
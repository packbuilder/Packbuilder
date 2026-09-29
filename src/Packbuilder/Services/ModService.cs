using Microsoft.EntityFrameworkCore;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class ModService(PackbuilderContext context) : IModService
    {
        public async Task<Mod> GetOrCreateModAsync(ExternalModSummary modSummary)
        {
            Mod? mod = await context.Mods.SingleOrDefaultAsync(m => m.ReferenceId == modSummary.ReferenceId && m.Platform == modSummary.Platform);

            if (mod is null)
            {
                mod = new()
                {
                    Platform = modSummary.Platform,
                    ReferenceId = modSummary.ReferenceId,
                    Name = modSummary.Name
                };

                context.Mods.Add(mod);
            }
            else if (mod.Name != modSummary.Name)
            {
                mod.Name = modSummary.Name;
            }

            return mod;
        }
    }
}
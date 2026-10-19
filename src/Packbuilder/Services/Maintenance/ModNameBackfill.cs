using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class ModNameBackfill(PackbuilderContext context, IExternalModService externalModService)
    {
        public async Task ExecuteAsync()
        {
            //TODO: Fetch mod data for other platforms when support for them is added
            List<string> curseforgeModIds = await context.Mods
                .Where(m => (m.Name == null || m.Name == "") && m.Platform == ModPlatform.CurseForge)
                    .Select(m => m.ReferenceId).ToListAsync();

            List<ExternalModSummary> curseForgeModData = await externalModService.GetExternalModsAsync(ModPlatform.CurseForge, curseforgeModIds);

            foreach (ExternalModSummary modSummary in curseForgeModData)
            {
                Mod? mod = await context.Mods.Where(m => m.ReferenceId == modSummary.ReferenceId).SingleOrDefaultAsync();

                if(mod is null)
                {
                    continue;
                }

                mod.Name = modSummary.Name;
                context.Mods.Update(mod);
            }

            await context.SaveChangesAsync();
        }
    }
}
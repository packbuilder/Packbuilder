using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.Interfaces;
using Packbuilder.Dto.ServiceDtos;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using Packbuilder.Models.enums;

namespace Packbuilder.Services
{
    public class ExternalModService(ICurseForgeModService curseForgeModService) : IExternalModService
    {     
        public async Task<ExternalModSummary> GetExternalModAsync(ModPlatform modPlatform, string referenceId)
        {
            switch (modPlatform)
            {
                case ModPlatform.CurseForge:
                    CurseForgeMod curseForgeMod = await curseForgeModService.GetModWithCacheAsync(referenceId);   
                    
                    return new()
                    {
                        Platform = ModPlatform.CurseForge,
                        ReferenceId = curseForgeMod.ReferenceId,
                        Name = curseForgeMod.Name
                    };

                default: throw new NotSupportedException($"Mod platform {modPlatform} is not supported.");
            }
        }

        public async Task<List<ExternalModSummary>> GetExternalModsAsync(ModPlatform modPlatform, List<string> referenceIds)
        {
            switch (modPlatform)
            {
                case ModPlatform.CurseForge:
                    List<CurseForgeMod> curseForgeMods = await curseForgeModService.GetModsWithCacheAsync(referenceIds);

                    List<ExternalModSummary> modSummaries =  curseForgeMods.Select(m => new ExternalModSummary()
                    {
                        Platform = modPlatform,
                        ReferenceId = m.ReferenceId,
                        Name = m.Name
                    }).ToList();
                    
                    return modSummaries;

                default: throw new NotSupportedException($"Mod platform {modPlatform} is not supported.");
            }
        }
    }
}

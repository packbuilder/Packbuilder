using CurseForge.Dtos;
using CurseForge.Dtos.CurseForgeApiDtos;
using CurseForge.enums;
using CurseForge.Interfaces;

namespace CurseForge.Services
{
    public class CurseForgeService(ICurseForgeApiService curseForgeApiService) : ICurseForgeService
    {
        public async Task<MissingDependenciesResultDto> CheckForMissingDependencies(string gameVersion, MinecraftModLoader modLoader, List<string> modReferenceIds)
        {
            HashSet<string> originalReferenceIds = modReferenceIds.ToHashSet();
            HashSet<string> missingDependencies = new();
            HashSet<string> visited = new();
            HashSet<string> modsWithIncompatibleDependencies = new();

            Stack<CurseForgeDependencyRelation> stack = new(modReferenceIds
                .Select(id => new CurseForgeDependencyRelation
                {
                    ReferenceId = id,
                    ParentReferenceId = null
                })
            );

            while (stack.Count > 0)
            {
                CurseForgeDependencyRelation modRelation = stack.Pop();

                if (!visited.Add(modRelation.ReferenceId))
                {
                    continue;
                }

                CurseForgeFile? latestModFile = await curseForgeApiService.GetLatestFileAsync(modRelation.ReferenceId, gameVersion, modLoader); 

                if(latestModFile is null)
                {
                    Console.WriteLine($"Mod with reference id: {modRelation.ReferenceId} does not exist for minecraft version {gameVersion} under mod loader {modLoader}");

                    // Should never be null as all parent mods sent should already be deemed compatible
                    if(modRelation.ParentReferenceId is not null)
                    {
                        modsWithIncompatibleDependencies.Add(modRelation.ParentReferenceId);
                    } else
                    {
                        throw new Exception($"Mod with reference id: {modRelation.ReferenceId} is an incompatible parent mod and should not be in the list of mod ids sent to this function.");
                    }

                    continue;
                }

                List<string> requiredDependencies = latestModFile.Dependencies
                    .Where(dependency => dependency.RelationType == 3)
                        .Select(d => d.ModReferenceId.ToString()).ToList();

                foreach (string dependencyReferenceId in requiredDependencies)
                {
                    CurseForgeDependencyRelation newModRelation = new()
                    {
                        ReferenceId = dependencyReferenceId,
                        ParentReferenceId = modRelation.ReferenceId
                    };

                    stack.Push(newModRelation);
                }

                if (!originalReferenceIds.Contains(modRelation.ReferenceId))
                {
                    missingDependencies.Add(modRelation.ReferenceId);
                }
            }

            return new()
            {
                MissingDependencies = missingDependencies,
                ModsWithIncompatibleDependencies = modsWithIncompatibleDependencies
            };
        }

        public async Task<CurseForgeModLoaderSummary?> GetOptimalModLoaderVersion(string minecraftVersion, MinecraftModLoader minecraftModLoader)
        {
            List<CurseForgeModLoaderSummary>? res = await curseForgeApiService.GetMinecraftModLoaders(minecraftVersion) ?? throw new Exception("Problem with fetching minecraft mod loaders from curseforge.");

            CurseForgeModLoaderSummary? optimalModLoaderVersion = res.FirstOrDefault(m => m.Type == minecraftModLoader && m.GameVersion == minecraftVersion && m.Recommended == true) ?? res.FirstOrDefault(m => m.Type == minecraftModLoader && m.GameVersion == minecraftVersion && m.Latest == true);

            return optimalModLoaderVersion;
        }
    
        public async Task<ModCompatibilityResultDto?> CheckModCompatibility(string gameVersion, MinecraftModLoader modLoader, List<string> modReferenceIds)
        {
            ModCompatibilityResultDto result = new()
            {
                CompatibleMods = [],
                InCompatibleMods = []
            };

            int? modClassId = await GetModClassIdAsync();

            foreach (string modId in modReferenceIds)
            {
                CurseForgeMod? modData = await curseForgeApiService.GetModAsync(modId);

                if(modData is null)
                {
                    result.InCompatibleMods.Add(modId);
                    continue;
                }

                bool isMod = modData.ClassId == modClassId;

                CurseForgeFile? latestFile = isMod ? await curseForgeApiService.GetLatestFileAsync(modId, gameVersion, modLoader) : await curseForgeApiService.GetLatestFileAsync(modId, gameVersion);

                if (latestFile is null || modData is null)
                {
                    result.InCompatibleMods.Add(modId);
                    continue;
                }

                result.CompatibleMods.Add(modId);
            }

            return result;
        } 

        private async Task<int?> GetModClassIdAsync()
        {
            List<CurseForgeCategory>? categories =  await curseForgeApiService.GetMinecraftCategories();

            if(categories is null)
            {
                return null;
            }

            return categories
                .FirstOrDefault(c => c.Slug == "mods")
                ?.Id;
        }
    }
}
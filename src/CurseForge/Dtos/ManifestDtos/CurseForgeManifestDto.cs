using CurseForge.enums;

namespace CurseForge.Dtos.ManifestDtos
{
    public class CurseForgeManifestDto
    {
        public required string Name { get; set; }
        public required string GameVersion { get; set; }
        public required MinecraftModLoader ModLoader { get; set; }
        public required List<CurseForgeManifestModDto> ModFiles { get; set; } 
    }
}
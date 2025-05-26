namespace CurseForge.Dtos
{
    public class ModCompatibilityResultDto
    {
        public required HashSet<string> CompatibleMods { get; set; } = [];
        public required HashSet<string> InCompatibleMods { get; set; } = [];
    }
}
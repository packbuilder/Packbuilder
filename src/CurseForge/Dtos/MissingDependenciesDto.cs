namespace CurseForge.Dtos
{
    public class MissingDependenciesResultDto
    {
        public required HashSet<string> MissingDependencies { get; set; }
        public required HashSet<string> ModsWithIncompatibleDependencies { get; set; }
    }
}
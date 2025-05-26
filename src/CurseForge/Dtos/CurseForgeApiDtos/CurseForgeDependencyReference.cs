namespace CurseForge.Dtos.CurseForgeApiDtos
{
    public class CurseForgeDependencyRelation
    {
        public required string ReferenceId { get; set; }
        public required string? ParentReferenceId { get; set; }
    }
}
namespace CurseForge.Models;

public class CurseForgeMod
{
    public required int ReferenceId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? LogoUrl { get; set; }
}

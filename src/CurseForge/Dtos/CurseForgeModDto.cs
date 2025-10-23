namespace CurseForge.Dtos;

public class CurseForgeModDto
{
    public required int ReferenceId { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public string? LogoUrl { get; set; }
}

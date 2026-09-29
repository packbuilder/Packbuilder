using Packbuilder.Models.enums;

public class ExternalModSummary
{
    public required string ReferenceId { get; set; }
    public required string Name { get; set; }
    public required ModPlatform Platform { get; set; }
}
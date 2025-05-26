namespace CurseForge.Options;

public sealed record CurseForgeApiOptions
{
    public required string BaseUrl { get; init; }
    public required string ApiKey { get; init; }
}
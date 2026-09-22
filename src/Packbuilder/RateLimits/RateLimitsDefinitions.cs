namespace Packbuilder.RateLimits;
public static class RateLimitDefinitions
{
    private static readonly Dictionary<string, RateLimitDefinition> Definitions = new()
    {
        [RateLimitBuckets.Auth] = new(20, 60),
        [RateLimitBuckets.Jobs] = new(10, 60),
        [RateLimitBuckets.Downloads] = new(10, 60),
        [RateLimitBuckets.Uploads] = new(5, 60),
        [RateLimitBuckets.Curseforge] = new(120, 60),
        [RateLimitBuckets.ModpackRead] = new(120, 60),
        [RateLimitBuckets.ModpackWrite] = new(60, 60),
        [RateLimitBuckets.ProfileWrite] = new(20, 60)
    };

    public static RateLimitDefinition? Get(string? bucket) => bucket is not null ? Definitions[bucket] : null;
}
public record RateLimitDefinition
(
    int Limit, 
    int WindowSeconds 
);
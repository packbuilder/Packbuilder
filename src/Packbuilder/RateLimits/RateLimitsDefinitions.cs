namespace Packbuilder.RateLimits;
public static class RateLimitDefinitions
{
    private static readonly Dictionary<string, RateLimitDefinition> Definitions = new()
    {
        [RateLimitBuckets.Auth] = new(5, 60),
        [RateLimitBuckets.Jobs] = new(5, 60),
        [RateLimitBuckets.Downloads] = new(5, 60),
        [RateLimitBuckets.Uploads] = new(3, 60),
        [RateLimitBuckets.Curseforge] = new(120, 60),
        [RateLimitBuckets.ModpackRead] = new(120, 60),
        [RateLimitBuckets.ModpackWrite] = new(60, 60),
        [RateLimitBuckets.ProfileWrite] = new(10, 60)
    };

    public static RateLimitDefinition Get(string bucket) =>
        Definitions[bucket];
}
public record RateLimitDefinition
(
    int Limit, 
    int WindowSeconds 
);
namespace Packbuilder.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RateLimitAttribute(string? bucket = null) : Attribute
{
    public string? Bucket { get; } = bucket;
}
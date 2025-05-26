namespace Packbuilder.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class RateLimitAttribute(string bucket) : Attribute
{
    public string Bucket { get; } = bucket;
}
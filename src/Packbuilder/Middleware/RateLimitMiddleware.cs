using System.Security.Claims;
using Packbuilder.Attributes;
using Packbuilder.RateLimits;
using StackExchange.Redis;

namespace Packbuilder.Middleware;

public sealed class RateLimitMiddleware(IConnectionMultiplexer redis) : IMiddleware
{
    private readonly IConnectionMultiplexer _redis = redis;

    private const int _generalRateLimitCount = 100;
    private const int _generalRateLimitWindowSeconds = 60;

    public async Task InvokeAsync(
        HttpContext context,
        RequestDelegate next)
    {
        IDatabase db = _redis.GetDatabase();

        (string key, int limit, int windowSeconds) = GetRateLimitKey(context);

        long count = await db.StringIncrementAsync(key);

        if (count == 1)
        {
            await db.KeyExpireAsync(
                key,
                TimeSpan.FromSeconds(windowSeconds));
        }

        if (count > limit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "Rate limit exceeded"
            });

            return;
        }

        await next(context);
    }

   private static (string Key, int Limit, int WindowSeconds) GetRateLimitKey(HttpContext context)
    {
        Endpoint? endpoint = context.GetEndpoint();

        RateLimitAttribute? rateLimitAttribute = endpoint?.Metadata.GetMetadata<RateLimitAttribute>();

        RateLimitDefinition? rateLimit = RateLimitDefinitions.Get(rateLimitAttribute?.Bucket); 

        if (rateLimitAttribute != null && rateLimit is not null)
        {
            return ($"rate:{rateLimitAttribute.Bucket}:{GetIdentifier(context)}", rateLimit.Limit, rateLimit.WindowSeconds);
        }

        return (
            $"rate:general:{GetIdentifier(context)}",
            _generalRateLimitCount , _generalRateLimitWindowSeconds
        );
    }

    private static string GetIdentifier(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            return context.User.FindFirstValue(
                ClaimTypes.NameIdentifier) ?? "unknownUser";
        }

        return context.Request.Headers["CF-Connecting-IP"]
            .FirstOrDefault()
            ?? context.Connection.RemoteIpAddress?.ToString()
            ?? "unknownIp";
    }
}
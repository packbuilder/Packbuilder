using Packbuilder.Dto.ModpackDtos;
using Packbuilder.Events;
using Packbuilder.Interfaces;
using Packbuilder.Models.enums;
using StackExchange.Redis;

namespace Packbuilder.Jobs;

public class VerificationJob(IConnectionMultiplexer redis, IServiceScopeFactory scopeFactory, IEventDispatcher eventDispatcher) : IHostedService
{
    public static readonly RedisChannel ChannelName = RedisChannel.Literal("Suggestions");
    private static readonly TimeSpan LockTtl = TimeSpan.FromMinutes(5);
    private ChannelMessageQueue? _channel = null;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await redis.GetSubscriber().SubscribeAsync(ChannelName);

        _channel.OnMessage(message =>
        {
            _ = HandleMessageAsync(message);
        });
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel is not null)
        {
            await _channel.UnsubscribeAsync();
        }
    }

    private async Task HandleMessageAsync(ChannelMessage message)
    {
        Console.WriteLine("Handling message");
        IDatabase db = redis.GetDatabase(); 
        int suggestionId = (int)message.Message;
        string lockKey = $"verify:{suggestionId}";
        bool acquired = await redis.GetDatabase().StringSetAsync(lockKey, "1", LockTtl, when: When.NotExists);

        if (!acquired)
            return; 
        
        using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        ISuggestionService suggestionService = scope.ServiceProvider.GetRequiredService<ISuggestionService>();

        SuggestionDto suggestionDto = await suggestionService.GetSuggestionById(suggestionId) ?? throw new Exception($"Suggestion with id {suggestionId} does not exist");
    
        try
        {
            await suggestionService.VerifyMinecraftSuggestionAsync(suggestionId);
            await eventDispatcher.PublishAsync<SuggestionUpdatedEvent>(new(suggestionDto.ModpackId, suggestionId));
        }
        catch (Exception exception)
        {
            await suggestionService.UpdateSuggestionState(suggestionId, SuggestionState.Unverified);
            Console.WriteLine(exception.Message);
            throw;
        }
        finally
        {
            await db.KeyDeleteAsync(lockKey);
        }
    }
}


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

        _channel.OnMessage(async message =>
        {
            try
            {
                await HandleMessageAsync(message);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
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
        IDatabase db = redis.GetDatabase(); 
        int suggestionId = (int)message.Message;
        string lockKey = $"verify:{suggestionId}";
        bool acquired = await redis.GetDatabase().StringSetAsync(lockKey, "1", LockTtl, when: When.NotExists);

        if (!acquired)
        {
            Console.WriteLine("-----------------------");
            Console.WriteLine($"Already aquired key for suggestion Id {suggestionId}, will not run job");
            Console.WriteLine("-----------------------");
            return; 
        }

        try
        {
            using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
            ISuggestionService suggestionService = scope.ServiceProvider.GetRequiredService<ISuggestionService>();

            SuggestionDto suggestionDto = await suggestionService.GetSuggestionById(suggestionId) ?? throw new Exception($"Suggestion with id {suggestionId} does not exist");
            
            await suggestionService.VerifyMinecraftSuggestionAsync(suggestionId);
            await eventDispatcher.PublishAsync<SuggestionUpdatedEvent>(new(suggestionDto.ModpackId, suggestionId));
        }
        catch (Exception exception)
        {
            using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
            ISuggestionService suggestionService = scope.ServiceProvider.GetRequiredService<ISuggestionService>();

            await suggestionService.UpdateSuggestionState(suggestionId, SuggestionState.Unverified);

            Console.WriteLine("-----------------------");
            Console.WriteLine(exception.Message);
            Console.WriteLine("-----------------------");

            throw;
        }
        finally
        {
            await db.KeyDeleteAsync(lockKey);
        }
    }
}


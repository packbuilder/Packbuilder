using Packbuilder.Dto;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using StackExchange.Redis;

namespace Packbuilder.Jobs;
public class SendPasswordResetEmailJob(IConnectionMultiplexer redis, IServiceScopeFactory scopeFactory) : IHostedService
{
    public static readonly RedisChannel ChannelName = RedisChannel.Literal("PasswordResetEmail");
    private static readonly TimeSpan LockTtl = TimeSpan.FromMinutes(1);
    private ChannelMessageQueue? _channel = null;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _channel = await redis.GetSubscriber().SubscribeAsync(ChannelName);

        _channel.OnMessage(message =>
        {
            _ = Task.Run(async () =>
            {
                await HandleMessageAsync(message);
            });
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
        int userId = (int)message.Message;
        string lockKey = $"password_reset_sent:{userId}";
        bool acquired = await db.StringSetAsync(lockKey, "1", LockTtl, when: When.NotExists);

        if (!acquired)
            return; 
        
        using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        ITokenService tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        ICacheService cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

        UserDto? user = await userService.GetUserById(userId) ?? throw new Exception("Unable to find user.");
        string rawToken = await cacheService.SetPasswordResetToken(userId);

        try
        {   
            await emailService.SendPasswordResetEmail(user.Email, rawToken, userId);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception.Message);
            throw;
        }
        finally
        {
            Console.WriteLine("Email successfully sent.");
        }
    }
}
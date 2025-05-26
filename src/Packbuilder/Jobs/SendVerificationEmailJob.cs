using Packbuilder.Dto;
using Packbuilder.Interfaces;
using Packbuilder.Models;
using StackExchange.Redis;

namespace Packbuilder.Jobs;
public class SendVerificationEmailJob(IConnectionMultiplexer redis, IServiceScopeFactory scopeFactory) : IHostedService
{
    public static readonly RedisChannel ChannelName = RedisChannel.Literal("VerficationEmail");
    private static readonly TimeSpan LockTtl = TimeSpan.FromMinutes(2);
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

        if (!int.TryParse(message.Message.ToString(), out int userId))
        {
            Console.WriteLine($"Invalid userId: {message.Message}");
            return;
        }
        
        string lockKey = $"email_verification_sent:{userId}";
        bool acquired = await db.StringSetAsync(lockKey, "1", LockTtl, when: When.NotExists);

        if (!acquired)
            return; 
        
        using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        IEmailService emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
        ITokenService tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();
        IUserService userService = scope.ServiceProvider.GetRequiredService<IUserService>();
        ICacheService cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();

        UserDto? user = await userService.GetUserById(userId) ?? throw new Exception("Unable to find user.");
        string rawVerificationCode = await cacheService.SetEmailVerificationToken(userId);

        try
        {   
            await emailService.SendVerificationEmail(user.Email, rawVerificationCode, userId);
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


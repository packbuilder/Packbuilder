using Microsoft.AspNetCore.SignalR;
using Packbuilder.Interfaces;

namespace Packbuilder.Events;

public class EventDispatcher(IServiceScopeFactory scopeFactory) : IEventDispatcher
{
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    public async Task PublishAsync<T>(T @event)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();

        IEventHandler<T> handler = scope.ServiceProvider.GetRequiredService<IEventHandler<T>>();

        await handler.Handle(@event);
    }
}
using Microsoft.AspNetCore.SignalR;
using Packbuilder.Events.Hubs;
using Packbuilder.Interfaces;

namespace Packbuilder.Events.Handlers;
public class ModpackUpdatedHandler(IHubContext<ModpackHub> hub) : IEventHandler<ModpackUpdatedEvent>
{
    private readonly IHubContext<ModpackHub> _hub = hub; 

    public Task Handle(ModpackUpdatedEvent @event)
    { 
        return _hub.Clients
            .Group($"modpack-{@event.ModpackId}")
            .SendAsync("ModpackUpdated", @event);
    }
}
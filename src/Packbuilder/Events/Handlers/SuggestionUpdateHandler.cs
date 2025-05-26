using Microsoft.AspNetCore.SignalR;
using Packbuilder.Events.Hubs;
using Packbuilder.Interfaces;
using Packbuilder.Models;

namespace Packbuilder.Events.Handlers;
public class SuggestionUpdatedHandler(IHubContext<ModpackHub> hub) : IEventHandler<SuggestionUpdatedEvent>
{
    private readonly IHubContext<ModpackHub> _hub = hub; 

    public Task Handle(SuggestionUpdatedEvent @event)
    { 
        return _hub.Clients
            .Group($"modpack-{@event.ModpackId}")
            .SendAsync("SuggestionUpdated", @event);
    }
}
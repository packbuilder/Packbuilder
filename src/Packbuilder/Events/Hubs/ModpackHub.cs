using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Packbuilder.Events.Hubs;

[Authorize(Policy = "VerifiedEmail")]
public class ModpackHub : Hub
{
    public Task JoinModpack(string modpackId)
    {
        return Groups.AddToGroupAsync(Context.ConnectionId, GetGroupName(modpackId));
    }

    public Task LeaveModpack(string modpackId)
    {
        return Groups.RemoveFromGroupAsync(Context.ConnectionId, GetGroupName(modpackId));
    }

    private static string GetGroupName(string modpackId)
        => $"modpack-{modpackId}";
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Infrastructure.Hubs
{
    public sealed class MessageHub : Hub
    {
        public void joinGroupHub(string groupId)
        {
            Groups.AddToGroupAsync(Context.ConnectionId, groupId);
            Clients.Client(Context.ConnectionId).SendAsync("clientMethodName", $"You joined to group hub successfully");
        }

        public override Task OnConnectedAsync()
        {
            var connectionId = Context.ConnectionId;
            Clients.Client(connectionId).SendAsync("WelcomeMethodName", connectionId);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            return base.OnDisconnectedAsync(exception);
        }
    }
}

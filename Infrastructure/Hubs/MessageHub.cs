using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Infrastructure.Hubs
{
    public sealed class MessageHub : Hub
    {
        public void joinGroupHub(int groupId)
        {
            var connectionId = Context.ConnectionId;
            Groups.AddToGroupAsync(connectionId, groupId.ToString());
            Clients.Client(connectionId).SendAsync("joinGroup", $"You joined to group hub successfully");
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

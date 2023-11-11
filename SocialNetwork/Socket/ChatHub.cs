namespace SocialNetwork.Socket
{
    using Microsoft.AspNetCore.SignalR;

    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task JoinGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("GroupMessage", $"{Context.ConnectionId} has joined the group {groupName}");
        }

        public async Task LeaveGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("GroupMessage", $"{Context.ConnectionId} has left the group {groupName}");
        }

        public async Task SendGroupMessage(string groupName, string user, string message)
        {
            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", user, message);
        }
    }



}

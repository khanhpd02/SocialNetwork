namespace SocialNetwork.Socket
{
    using Microsoft.AspNetCore.SignalR;
    using SocialNetwork.Service;

    public class ChatHub : Hub
    {
        private IGeneralService _generalService;
        public ChatHub(IGeneralService generalService)
        {
            _generalService = generalService;
        }
        private static Dictionary<string, string> userConnections = new Dictionary<string, string>();

        public async Task SendToUser(string userId, string message)
        {
            if (userConnections.TryGetValue(userId, out var receiverConnectionId))
            {
                await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", userId, message);
            }
            else
            {
                // Handle the case where the user is not connected
                // (you may want to send a different message or take other actions)
            }
        }

        public override Task OnConnectedAsync()
        {
            // Get the userId from your authentication system or other source
            var userId = _generalService.UserId.ToString(); // Replace this with your actual way of getting userId

            if (!string.IsNullOrEmpty(userId))
            {
                // Map the userId to the connectionId
                userConnections[userId] = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            // Remove the mapping when a connection is disconnected
            var userId = userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                userConnections.Remove(userId);
            }

            return base.OnDisconnectedAsync(exception);
        }
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
        public string GetConnectionId() => Context.ConnectionId;
    }

}

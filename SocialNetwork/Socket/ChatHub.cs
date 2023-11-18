namespace SocialNetwork.Socket
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using SocialNetwork.Middlewares;
    using SocialNetwork.Service;
    [TypeFilter(typeof(AuthenticationFilter))]

    public class ChatHub : Hub
    {
        private readonly IUserService _userService;

        public ChatHub(IUserService userService)
        {
            _userService = userService;
        }
        private static Dictionary<string, string> userConnections = new Dictionary<string, string>();

        public async Task SendToUser(string userId, string message)
        {
            try
            {
                if (userConnections.TryGetValue(userId, out var receiverConnectionId))
                {
                    await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", userId, message);
                }
                else
                {

                    Console.WriteLine($"User with ID {userId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Exception in SendToUser: {ex.Message}");
                throw;
            }
        }

        public override Task OnConnectedAsync()
        {
            var userId = _userService.UserEmail;

            if (!string.IsNullOrEmpty(userId))
            {
                userConnections[userId] = Context.ConnectionId;
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
            if (userId != null)
            {
                userConnections.Remove(userId);
            }

            return base.OnDisconnectedAsync(exception);
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

        public async Task SendGroupMessage(string groupName, string message)
        {

            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", _userService.UserEmail, message);
        }
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", _userService.UserEmail, message);
        }

        public string GetConnectionId() => Context.ConnectionId;
    }

}

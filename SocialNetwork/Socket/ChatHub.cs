namespace SocialNetwork.Socket
{
    using Microsoft.AspNetCore.SignalR;
    using SocialNetwork.Service;

    public class ChatHub : Hub
    {
        //#region test chat to user
        //private IGeneralService _generalService;
        //private static Dictionary<string, string> userConnections = new Dictionary<string, string>();
        //private static int userCounter = 1;

        //public ChatHub(IGeneralService generalService)
        //{
        //    _generalService = generalService;
        //}

        //public async Task SendToUser(string userId, string message)
        //{
        //    if (userConnections.TryGetValue(userId, out var receiverConnectionId))
        //    {
        //        await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", userId, message);
        //    }
        //    else
        //    {
        //    }
        //}

        //public override Task OnConnectedAsync()
        //{
        //    var userId = $"userID{userCounter}";

        //    if (!string.IsNullOrEmpty(userId))
        //    {
        //        userConnections[userId] = Context.ConnectionId;
        //    }

        //    userCounter++;

        //    return base.OnConnectedAsync();
        //}

        //public override Task OnDisconnectedAsync(Exception exception)
        //{
        //    var userId = userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        //    if (userId != null)
        //    {
        //        userConnections.Remove(userId);
        //    }

        //    return base.OnDisconnectedAsync(exception);
        //}
        //#endregion
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
            }
        }

        public override Task OnConnectedAsync()
        {
            var userId = _generalService.UserId.ToString();

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

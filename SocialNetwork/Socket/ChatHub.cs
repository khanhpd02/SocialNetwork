namespace SocialNetwork.Socket
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.SignalR;
    using SocialNetwork.Middlewares;
    using SocialNetwork.Service;
    [TypeFilter(typeof(AuthenticationFilter))]

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
        private readonly IGeneralService _generalService;
        private readonly IGeneralServiceFactory _generalServiceFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatHub(IGeneralService generalService, IGeneralServiceFactory generalServiceFactory, IHttpContextAccessor httpContextAccessor)
        {
            _generalService = generalService;
            _generalServiceFactory = generalServiceFactory;
            _httpContextAccessor = httpContextAccessor;
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
            var userId = _generalService.UserName;

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

            await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", _generalService.UserName, message);
        }
        public async Task SendMessage(string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", _httpContextAccessor.HttpContext?.User.FindFirst("id")?.Value, message);
        }

        public string GetConnectionId() => Context.ConnectionId;
    }

}

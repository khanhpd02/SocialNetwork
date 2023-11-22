//using AutoMapper;
//using Chat.Web.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using SocialNetwork.Entity;
//using SocialNetwork.Middlewares;
//using System.Text.RegularExpressions;

//namespace SocialNetwork.Socket
//{
//    [TypeFilter(typeof(AuthenticationFilter))]

//    //public class ChatHub : Hub
//    //{
//    //    private readonly IUserService _userService;

//    //    public ChatHub(IUserService userService)
//    //    {
//    //        _userService = userService;
//    //    }
//    //    private static Dictionary<string, string> userConnections = new Dictionary<string, string>();

//    //    public async Task SendToUser(string userId, string message)
//    //    {
//    //        try
//    //        {
//    //            if (userConnections.TryGetValue(userId, out var receiverConnectionId))
//    //            {
//    //                await Clients.Client(receiverConnectionId).SendAsync("ReceivePrivateMessage", userId, message);
//    //            }
//    //            else
//    //            {

//    //                Console.WriteLine($"User with ID {userId} not found.");
//    //            }
//    //        }
//    //        catch (Exception ex)
//    //        {
//    //            Console.Error.WriteLine($"Exception in SendToUser: {ex.Message}");
//    //            throw;
//    //        }
//    //    }

//    //    public override Task OnConnectedAsync()
//    //    {
//    //        var userEmail = GetCurrentUserEmail();

//    //        if (!string.IsNullOrEmpty(userEmail))
//    //        {
//    //            userConnections[userEmail] = Context.ConnectionId;
//    //        }

//    //        return base.OnConnectedAsync();
//    //    }
//    //    private string GetCurrentUserEmail()
//    //    {
//    //        var httpContext = Context.GetHttpContext();
//    //        var userEmailClaim = httpContext.User?.Claims.FirstOrDefault(c => c.Type == "email");

//    //        return userEmailClaim?.Value;
//    //    }

//    //    public override Task OnDisconnectedAsync(Exception exception)
//    //    {
//    //        var userId = userConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
//    //        if (userId != null)
//    //        {
//    //            userConnections.Remove(userId);
//    //        }

//    //        return base.OnDisconnectedAsync(exception);
//    //    }
//    //    public async Task JoinGroup(string groupName)
//    //    {
//    //        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
//    //        await Clients.Group(groupName).SendAsync("GroupMessage", $"{Context.ConnectionId} has joined the group {groupName}");
//    //    }

//    //    public async Task LeaveGroup(string groupName)
//    //    {
//    //        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
//    //        await Clients.Group(groupName).SendAsync("GroupMessage", $"{Context.ConnectionId} has left the group {groupName}");
//    //    }

//    //    public async Task SendGroupMessage(string groupName, string message)
//    //    {

//    //        await Clients.Group(groupName).SendAsync("ReceiveGroupMessage", _userService.UserEmail, message);
//    //    }
//    //    public async Task SendMessage(string message)
//    //    {
//    //        await Clients.All.SendAsync("ReceiveMessage", GetCurrentUserEmail(), message);
//    //    }

//    //    public string GetConnectionId() => Context.ConnectionId;
//    //}


//    [Authorize]
//    public class ChatHub : Hub
//    {
//        public readonly static List<UserViewModel> _Connections = new List<UserViewModel>();
//        private readonly static Dictionary<string, string> _ConnectionsMap = new Dictionary<string, string>();

//        private readonly SocialNetworkContext _context;
//        private readonly IMapper _mapper;

//        public ChatHub(SocialNetworkContext context, IMapper mapper)
//        {
//            _context = context;
//            _mapper = mapper;
//        }

//        public async Task SendPrivate(string receiverName, string message)
//        {
//            if (_ConnectionsMap.TryGetValue(receiverName, out string userId))
//            {
//                // Who is the sender;
//                var sender = _Connections.Where(u => u.UserName == IdentityName).First();

//                if (!string.IsNullOrEmpty(message.Trim()))
//                {
//                    // Build the message
//                    var messageViewModel = new MessageViewModel()
//                    {
//                        Content = Regex.Replace(message, @"<.*?>", string.Empty),
//                        FromUserName = sender.UserName,
//                        FromFullName = sender.FullName,
//                        Avatar = sender.Avatar,
//                        Room = "",
//                        Timestamp = DateTime.Now
//                    };

//                    // Send the message
//                    await Clients.Client(userId).SendAsync("newMessage", messageViewModel);
//                    await Clients.Caller.SendAsync("newMessage", messageViewModel);
//                }
//            }
//        }

//        public async Task Join(string roomName)
//        {
//            try
//            {
//                var user = _Connections.Where(u => u.UserName == IdentityName).FirstOrDefault();
//                if (user != null && user.CurrentRoom != roomName)
//                {
//                    // Remove user from others list
//                    if (!string.IsNullOrEmpty(user.CurrentRoom))
//                        await Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);

//                    // Join to new chat room
//                    await Leave(user.CurrentRoom);
//                    await Groups.AddToGroupAsync(Context.ConnectionId, roomName);
//                    user.CurrentRoom = roomName;

//                    // Tell others to update their list of users
//                    await Clients.OthersInGroup(roomName).SendAsync("addUser", user);
//                }
//            }
//            catch (Exception ex)
//            {
//                await Clients.Caller.SendAsync("onError", "You failed to join the chat room!" + ex.Message);
//            }
//        }

//        public async Task Leave(string roomName)
//        {
//            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomName);
//        }

//        public IEnumerable<UserViewModel> GetUsers(string roomName)
//        {
//            return _Connections.Where(u => u.CurrentRoom == roomName).ToList();
//        }

//        public override Task OnConnectedAsync()
//        {
//            try
//            {
//                var user = _context.Users.Where(u => u.UserName == IdentityName).FirstOrDefault();
//                //var userViewModel = _mapper.Map<ApplicationUser, UserViewModel>(user);
//                userViewModel.Device = GetDevice();
//                userViewModel.CurrentRoom = "";

//                if (!_Connections.Any(u => u.UserName == IdentityName))
//                {
//                    _Connections.Add(userViewModel);
//                    _ConnectionsMap.Add(IdentityName, Context.ConnectionId);
//                }

//                Clients.Caller.SendAsync("getProfileInfo", userViewModel);
//            }
//            catch (Exception ex)
//            {
//                Clients.Caller.SendAsync("onError", "OnConnected:" + ex.Message);
//            }
//            return base.OnConnectedAsync();
//        }

//        public override Task OnDisconnectedAsync(Exception exception)
//        {
//            try
//            {
//                var user = _Connections.Where(u => u.UserName == IdentityName).First();
//                _Connections.Remove(user);

//                // Tell other users to remove you from their list
//                Clients.OthersInGroup(user.CurrentRoom).SendAsync("removeUser", user);

//                // Remove mapping
//                _ConnectionsMap.Remove(user.UserName);
//            }
//            catch (Exception ex)
//            {
//                Clients.Caller.SendAsync("onError", "OnDisconnected: " + ex.Message);
//            }

//            return base.OnDisconnectedAsync(exception);
//        }

//        private string IdentityName
//        {
//            get { return Context.User.Identity.Name; }
//        }

//        private string GetDevice()
//        {
//            var device = Context.GetHttpContext().Request.Headers["Device"].ToString();
//            if (!string.IsNullOrEmpty(device) && (device.Equals("Desktop") || device.Equals("Mobile")))
//                return device;

//            return "Web";
//        }
//    }
//}

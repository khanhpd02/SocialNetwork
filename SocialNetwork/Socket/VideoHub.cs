using Microsoft.AspNetCore.SignalR;

namespace SocialNetwork.Socket
{
    public class VideoHub : Hub
    {
        public static List<string> ConnectedUsers = new List<string>();

        public override Task OnConnectedAsync()
        {
            Clients.Client(Context.ConnectionId).SendAsync("RequestUsername");
            return base.OnConnectedAsync();
        }

        public async Task SetUsername(string username)
        {
            var existingUser = ConnectedUsers.SingleOrDefault(u => u.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (existingUser == null)
            {
                // Đặt username cho người dùng
                Context.Items["Username"] = username;
                ConnectedUsers.Add(username);
                await Clients.All.SendAsync("UpdateUserList", ConnectedUsers);
            }
            else
            {
                // Gửi thông báo về việc đặt username không thành công
                await Clients.Caller.SendAsync("UsernameExists", username);
            }
        }

        public async Task SendVideoCallRequest(string callerId, string calleeUsername)
        {
            var calleeId = ConnectedUsers.SingleOrDefault(u => u.Equals(calleeUsername, StringComparison.OrdinalIgnoreCase));
            if (calleeId != null)
            {
                await Clients.Client(calleeId).SendAsync("ReceiveVideoCallRequest", callerId);
            }
            else
            {
                // Gửi thông báo về việc người dùng không tồn tại hoặc không kết nối
                await Clients.Caller.SendAsync("UserNotAvailable", calleeUsername);
            }
        }

        public async Task AnswerVideoCall(string callerId, string calleeId)
        {
            await Clients.Client(callerId).SendAsync("StartVideoCall", calleeId);
        }
    }



}

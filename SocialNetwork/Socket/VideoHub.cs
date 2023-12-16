using Microsoft.AspNetCore.SignalR;

namespace SocialNetwork.Socket
{
    public class VideoHub : Hub
    {
        public async Task JoinCall(string username)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "VideoCall");
            await Clients.Group("VideoCall").SendAsync("UserJoined", username);
        }

        public async Task LeaveCall(string username)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "VideoCall");
            await Clients.Group("VideoCall").SendAsync("UserLeft", username);
        }

        public async Task SendSignal(string targetUser, string signal)
        {
            await Clients.User(targetUser).SendAsync("ReceiveSignal", Context.ConnectionId, signal);
        }
    }



}

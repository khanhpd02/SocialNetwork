using Microsoft.AspNetCore.SignalR;
using SocialNetwork.DTO;

namespace SocialNetwork.Socket
{

    public class CommentHub : Hub
    {
        public async Task SendComment(string userId, Guid postId, CommentDTO comment)
        {
            await Clients.Group(postId.ToString()).SendAsync("ReceiveComment", userId, comment);
        }

        public async Task JoinPostGroup(Guid postId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, postId.ToString());
        }

        public async Task LeavePostGroup(Guid postId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, postId.ToString());
        }
    }
}

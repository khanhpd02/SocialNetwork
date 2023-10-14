namespace SocialNetwork.Socket
{
    using Microsoft.AspNetCore.SignalR;
    using SocialNetwork.Controller;

    public class ChatHub : Hub
    {
        private static readonly List<ChatMessage> chatMessages = new List<ChatMessage>();

        public async Task SendMessage(string user, string message)
        {
            var chatMessage = new ChatMessage { User = user, Message = message };
            chatMessages.Add(chatMessage);

            await Clients.All.SendAsync("ReceiveMessage", chatMessage);

            // Additional code to limit the number of messages, if needed
            if (chatMessages.Count > 100)
            {
                chatMessages.RemoveAt(0);
            }
        }


    }



}

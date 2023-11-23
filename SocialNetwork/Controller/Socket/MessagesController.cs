//using AutoMapper;
//using Chat.Web.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;
//using Microsoft.EntityFrameworkCore;
//using SocialNetwork.Entity;
//using SocialNetwork.Socket;
//using System.Text.RegularExpressions;

//namespace SocialNetwork.Controller.Socket
//{
//    [Authorize]
//    [Route("api/[controller]")]
//    [ApiController]
//    public class MessagesController : ControllerBase
//    {
//        private readonly SocialNetworkContext _context;
//        private readonly IMapper _mapper;
//        private readonly IHubContext<ChatHub> _hubContext;

//        public MessagesController(SocialNetworkContext context,
//            IMapper mapper,
//            IHubContext<ChatHub> hubContext)
//        {
//            _context = context;
//            _mapper = mapper;
//            _hubContext = hubContext;
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<GroupChat>> Get(int id)
//        {
//            var message = await _context.Chats.FindAsync(id);
//            if (message == null)
//                return NotFound();

//            var messageViewModel = _mapper.Map<SocialNetwork.Entity.Chat, MessageViewModel>(message);
//            return Ok(messageViewModel);
//        }

//        [HttpGet("Room/{roomName}")]
//        public IActionResult GetMessages(string roomName)
//        {
//            var room = _context.GroupChats.FirstOrDefault(r => r.GroupName == roomName);
//            if (room == null)
//                return BadRequest();

//            var messages = _context.Chats.Where(m => m.ToRoomId == room.Id)
//                .Include(m => m.FromUser)
//                .Include(m => m.ToRoom)
//                .OrderByDescending(m => m.Timestamp)
//                .Take(20)
//                .AsEnumerable()
//                .Reverse()
//                .ToList();

//            var messagesViewModel = _mapper.Map<IEnumerable<Chat>, IEnumerable<MessageViewModel>>(messages);

//            return Ok(messagesViewModel);
//        }

//        [HttpPost]
//        public async Task<ActionResult<Chat>> Create(MessageViewModel viewModel)
//        {
//            var user = _context.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
//            var room = _context.Rooms.FirstOrDefault(r => r.Name == viewModel.Room);
//            if (room == null)
//                return BadRequest();

//            var msg = new Chat()
//            {
//                Content = Regex.Replace(viewModel.Content, @"<.*?>", string.Empty),
//                FromUser = user,
//                ToRoom = room,
//                Timestamp = DateTime.Now
//            };

//            _context.Chats.Add(msg);
//            await _context.SaveChangesAsync();

//            // Broadcast the message
//            var createdMessage = _mapper.Map<Chat, MessageViewModel>(msg);
//            await _hubContext.Clients.Group(room.Name).SendAsync("newMessage", createdMessage);

//            return CreatedAtAction(nameof(Get), new { id = msg.Id }, createdMessage);
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> Delete(int id)
//        {
//            var message = await _context.Chats
//                .Include(u => u.FromUser)
//                .Where(m => m.Id == id && m.FromUser.UserName == User.Identity.Name)
//                .FirstOrDefaultAsync();

//            if (message == null)
//                return NotFound();

//            _context.Chats.Remove(message);
//            await _context.SaveChangesAsync();

//            await _hubContext.Clients.All.SendAsync("removeChatMessage", message.Id);

//            return Ok();
//        }
//    }
//}

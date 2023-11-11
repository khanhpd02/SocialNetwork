using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.Service;
using SocialNetwork.Socket;

namespace SocialNetwork.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private IGeneralService _generalService;
        public ChatController(IHubContext<ChatHub> hubContext, IHttpContextAccessor httpContextAccessor, IGeneralService generalService)
        {
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            _generalService = generalService;
        }

        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] MessageModel messageModel)
        {
            if (ModelState.IsValid)
            {
                var connectionId = _httpContextAccessor.HttpContext.Connection.Id;
                await _hubContext.Clients.All.SendAsync("ReceiveMessage", _generalService.UserName, messageModel.Message);
                return Ok();
            }
            return BadRequest("Invalid message model");
        }

        [HttpPost("JoinGroup")]
        public async Task<IActionResult> JoinGroup([FromBody] GroupModel groupModel)
        {
            if (ModelState.IsValid)
            {
                var connectionId = _httpContextAccessor.HttpContext.Connection.Id;
                await _hubContext.Groups.AddToGroupAsync(connectionId, groupModel.GroupName);
                await _hubContext.Clients.Group(groupModel.GroupName).SendAsync("GroupMessage", $"{connectionId} has joined the group {groupModel.GroupName}");
                return Ok();
            }
            return BadRequest("Invalid group model");
        }

        [HttpPost("LeaveGroup")]
        public async Task<IActionResult> LeaveGroup([FromBody] GroupModel groupModel)
        {
            if (ModelState.IsValid)
            {
                var connectionId = _httpContextAccessor.HttpContext.Connection.Id;
                await _hubContext.Groups.RemoveFromGroupAsync(connectionId, groupModel.GroupName);
                await _hubContext.Clients.Group(groupModel.GroupName).SendAsync("GroupMessage", $"{connectionId} has left the group {groupModel.GroupName}");
                return Ok();
            }
            return BadRequest("Invalid group model");
        }

        [HttpPost("SendGroupMessage")]
        public async Task<IActionResult> SendGroupMessage([FromBody] GroupMessageModel groupMessageModel)
        {
            if (ModelState.IsValid)
            {
                await _hubContext.Clients.Group(groupMessageModel.GroupName).SendAsync("ReceiveGroupMessage", _generalService.UserName, groupMessageModel.Message);
                return Ok();
            }
            return BadRequest("Invalid group message model");
        }
    }

    public class MessageModel
    {
        public string Message { get; set; }
    }

    public class GroupModel
    {
        public string GroupName { get; set; }
    }

    public class GroupMessageModel
    {
        public string GroupName { get; set; }
        public string Message { get; set; }
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.DTO.Chat;
using SocialNetwork.Middlewares;
using SocialNetwork.Repository;
using SocialNetwork.Socket;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUserRepository userRepository;
        private readonly IInforRepository inforRepository;
        private readonly IGroupChatRepository groupChatRepository;
        private readonly IUserGroupChatRepository userGroupChatRepository;

        public ChatController(IHubContext<ChatHub> hubContext, IUserRepository userRepository, IInforRepository inforRepository
            , IGroupChatRepository groupChatRepository, IUserGroupChatRepository userGroupChatRepository)
        {
            _hubContext = hubContext;
            this.userRepository = userRepository;
            this.inforRepository = inforRepository;
            this.groupChatRepository = groupChatRepository;
            this.userGroupChatRepository = userGroupChatRepository;
        }

        //[HttpPost]
        //[SwaggerOperation(Summary = "Gửi tin nhắn")]
        //public async Task<IActionResult> SendMessage([FromBody] ChatMessageDTO message)
        //{
        //    string userEmail = Request.Cookies["UserEmail"];
        //    var user = userRepository.FindByCondition(x => x.Email == userEmail).FirstOrDefault();
        //    var infor = inforRepository.FindByCondition(x => x.UserId == user.Id).FirstOrDefault();
        //    message.User = infor.FullName;
        //    await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.User, message.Message);
        //    return Ok(message);
        //}
        [HttpPost]
        [SwaggerOperation(Summary = "Gửi tin nhắn")]
        public async Task<IActionResult> SendMessage([FromBody] ChatMessageDTO message)
        {
            string userEmail = Request.Cookies["UserEmail"];
            var user = userRepository.FindByCondition(x => x.Email == userEmail).FirstOrDefault();
            var infor = inforRepository.FindByCondition(x => x.UserId == user.Id).FirstOrDefault();
            message.User = infor.FullName;

            // Tham gia vào một group dựa trên logic của bạn
            string groupName = "";
            //var group = userGroupChatRepository.FindByCondition(x => x.UserId == user.Id && x.GroupChatId == message.groupId).ToList();
            //foreach (var item in group)
            //{
            //    var groupCheck = groupChatRepository.FindByCondition(x => x.Id == item.GroupChatId).FirstOrDefault();                                                                // Lấy ConnectionId từ HTTP header (giả sử header có tên là "ConnectionId")
            //    groupName = groupCheck.GroupName;
            //}
            string connectionId = Request.Headers["ConnectionId"];
            await _hubContext.Clients.Client(connectionId).SendAsync("JoinGroup", groupName);

            await _hubContext.Clients.Group(groupName).SendAsync("ReceiveMessage", message.User, message.Message);
            return Ok(message);
        }

    }

}

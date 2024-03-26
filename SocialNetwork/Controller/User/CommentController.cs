using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{

    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/cmt")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        private IUserService _userService;
        public CommentController(ICommentService commentService, IUserService userService)
        {
            this.commentService = commentService;
            _userService = userService;
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create Comment")]
        public Task<IActionResult> Create([FromForm] CommentDTO dto)
        {
            var response = commentService.create(dto);
            return Task.FromResult<IActionResult>(Ok(response));
        }
        [HttpGet("getcmtPost/{id}")]
        [SwaggerOperation(Summary = "Get All Comment on Post")]
        public IActionResult getAllOnPost(Guid id)
        {
            var response = commentService.getAllOnPost(id);
            return Ok(response);
        }
        [HttpPut("update")]
        [SwaggerOperation(Summary = "Update Comment")]
        public IActionResult Update([FromBody] CommentDTO dto)
        {
            AppResponse response = commentService.update(dto);
            return Ok(response);
        }

        [HttpGet("getmycmt")]
        [SwaggerOperation(Summary = "Get my CMT at all Post")]
        public IActionResult getlikeOfUser()
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);
            var response = commentService.getallofUser(userid);
            return Ok(response);
        }
        [HttpPost("deleteOrUndo/{id}")]
        [SwaggerOperation(Summary = "delete Or Undo CmT")]
        public IActionResult deleteOrUndo(Guid id)
        {
            var response = commentService.deleteOfUndo(id);
            return Ok(response);
        }
    }
}

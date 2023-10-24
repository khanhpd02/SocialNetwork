using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Cloudinary;
using SocialNetwork.DTO.Response;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using SocialNetwork.Service.Implement;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/cmt")]
    public class CommentController : ControllerBase
    {

        private readonly ICommentService commentService;

        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create Comment")]
        public IActionResult Create([FromBody]CommentDTO dto)
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);


            var response = commentService.create(dto, userid);

            return Ok(response);
        }
        [HttpPost("update")]
        [SwaggerOperation(Summary = "Update Comment")]
        public IActionResult Update([FromBody] CommentDTO dto)
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);


            AppResponse response = commentService.update(dto, userid);

            return Ok(response);
        }

        [HttpGet("getcmtPost/{id}")]
        [SwaggerOperation(Summary = "Get All Comment on Post")]
        public IActionResult getlikeoofuser(Guid id)
        {
            //Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);
            var response = commentService.getallOnPost(id);
            return Ok(response);
        }
        [HttpGet("getmycmt")]
        [SwaggerOperation(Summary = "Get my CMT at all Post")]
        public IActionResult getlikeoofuser()
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);
            var response = commentService.getallofUser(userid);
            return Ok(response);
        }
        [HttpPost("deleteOrUndo/{id}")]
        [SwaggerOperation(Summary = "delete Or Undo CmT")]
        public IActionResult deleteOrUndo(Guid id)
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);
            var response = commentService.deleteOfUndo(id,userid);
            return Ok(response);
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/infor")]
    public class InforController : ControllerBase
    {

        private readonly IInforService inforService;

        public InforController(IInforService inforService)
        {
            this.inforService = inforService;
        }

        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create Infor")]
        public IActionResult Create([FromForm] InforDTO dto)
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);


            var response = inforService.createInfo(dto, userid);

            return Ok(response);
        }
        [HttpPost("update")]
        [SwaggerOperation(Summary = "Update Infor")]
        public IActionResult Update([FromForm] InforDTO dto)
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);


            var response = inforService.updateInfo(dto, userid);

            return Ok(response);
        }
        [HttpGet("user/{id}")]
        [SwaggerOperation(Summary = "Get Infor By UserId")]
        public IActionResult GetInforByUserId(Guid id)
        {
            var infor = inforService.GetInforByUserId(id);
            if (infor == null)
            {
                return NotFound();
            }
            return Ok(infor);
        }
        [HttpGet("searchuser")]
        [SwaggerOperation(Summary = "Get Infor By Full Name")]
        public IActionResult GetInforByFullName(string fullname)
        {
            var infor = inforService.GetInforByFullName(fullname);
            if (infor == null)
            {
                return NotFound();
            }
            return Ok(infor);
        }
        [HttpGet("myinfor")]
        [SwaggerOperation(Summary = "Get My Infor")]
        public IActionResult GetMyInfor()
        {
            var infor = inforService.GetMyInfor();
            if (infor == null)
            {
                return NotFound();
            }
            return Ok(infor);
        }
        /* [HttpPut("update")]
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
             var response = commentService.deleteOfUndo(id, userid);
             return Ok(response);
         }*/
    }
}

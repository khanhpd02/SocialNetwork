using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Cloudinary;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using SocialNetwork.Service.Implement;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/like")]
    public class LikeController : ControllerBase
    {

        private readonly ILikeService _IlikeService;

        public LikeController(ILikeService ilikeService)
        {
            _IlikeService = ilikeService;
        }

        [HttpPost("{id}")]
        [SwaggerOperation(Summary = "Like or Unlike")]
        public IActionResult Create(Guid id)
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);

            
            var response= _IlikeService.LikeAndUnlike(id,userid);
            
            return Ok(response);
        }

        [HttpGet("likeonpost/{id}")]
        [SwaggerOperation(Summary = "Get All Posts By PostId")]
        public IActionResult getlikeoonpost(Guid id)
        {
            var response = _IlikeService.getallByPostId(id);
            return Ok(response);
        }
        [HttpGet("getmylike")]
        [SwaggerOperation(Summary = "Get All Posts By PostId")]
        public IActionResult getlikeoofuser()
        {
            Guid userid = Guid.Parse(HttpContext.User.FindFirst("id").Value);
            var response = _IlikeService.getallByUserID(userid);
            return Ok(response);
        }

    }
}

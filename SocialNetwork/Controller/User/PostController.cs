using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Cloudinary;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/post")]
    public class PostController : ControllerBase
    {

        private readonly IPostService postService;

        public PostController(IPostService postService)
        {
            this.postService = postService;
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Post by ID")]
        public IActionResult GetById(Guid id)
        {
            var post = postService.GetById(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a Post")]
        public IActionResult Create([FromBody] PostDTO postDTO)
        {
            string userEmail = Request.Cookies["UserEmail"];

            if (postDTO == null)
            {
                return BadRequest("Invalid data");
            }
            string cloudinaryUrl = Request.Cookies["CloudinaryUrl"];
            var createdPost = postService.Create(postDTO, userEmail, cloudinaryUrl);
            Response.Cookies.Delete("CloudinaryUrl");
            return Ok(createdPost);
        }
        [HttpPut]
        [SwaggerOperation(Summary = "Create a Post")]
        public IActionResult Update([FromBody] PostDTO postDTO)
        {
            string userEmail = Request.Cookies["UserEmail"];

            if (postDTO == null)
            {
                return BadRequest("Invalid data");
            }
            var createdPost = postService.Update(postDTO, userEmail);
            return Ok(createdPost);
        }
        [HttpPost("upload")]
        [SwaggerOperation(Summary = "Upload File to Cloudinary")]
        public IActionResult UploadFileToCloudinary([FromForm] FileUploadDTO file)
        {
            if (file == null)
            {
                return BadRequest("Invalid file");
            }

            // Gọi hàm tải lên tệp lên Cloudinary
            string cloudinaryUrl = postService.UploadFileToCloudinary(file);

            if (cloudinaryUrl != null)
            {
                Response.Cookies.Append("CloudinaryUrl", cloudinaryUrl);
                return Ok(new { CloudinaryUrl = cloudinaryUrl });
            }

            return BadRequest("Failed to upload file to Cloudinary");
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete Post by ID")]
        public IActionResult Delete(Guid id)
        {
            postService.Delete(id);
            return NoContent();
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All Posts")]
        public IActionResult GetAll()
        {
            var posts = postService.GetAll();
            return Ok(posts);
        }
    }
}

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
        private IGeneralService _generalService;
        public PostController(IPostService postService, IGeneralService generalService)
        {
            this.postService = postService;
            _generalService = generalService;
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

            if (postDTO == null)
            {
                return BadRequest("Invalid data");
            }
            var createdPost = postService.Create(postDTO);
            return Ok(createdPost);
        }
        [HttpPut]
        [SwaggerOperation(Summary = "Create a Post")]
        public IActionResult Update([FromBody] PostDTO postDTO)
        {

            if (postDTO == null)
            {
                return BadRequest("Invalid data");
            }
            var createdPost = postService.Update(postDTO);
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
                _generalService.CloudinaryUrl = cloudinaryUrl;
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

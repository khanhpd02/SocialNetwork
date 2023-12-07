using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SocialNetwork.DTO;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using SocialNetwork.Socket;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/post")]
    public class PostController : ControllerBase
    {
        private readonly IHubContext<CommentHub> _commentHub;

        private readonly IPostService postService;
        public PostController(IPostService postService, IHubContext<CommentHub> commentHub)
        {
            this.postService = postService;
            _commentHub = commentHub;
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
        public IActionResult Create([FromForm] PostDTO postDTO)
        {

            if (postDTO == null)
            {
                return BadRequest("Invalid data");
            }
            var createdPost = postService.Create(postDTO);
            return Ok(createdPost);
        }
        [HttpPut]
        [SwaggerOperation(Summary = "Update a Post")]
        public IActionResult Update([FromForm] PostDTO postDTO)
        {

            if (postDTO == null)
            {
                return BadRequest("Invalid data");
            }
            var createdPost = postService.Update(postDTO);
            return Ok(createdPost);
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
            foreach (var post in posts)
            {
                _commentHub.Clients.Group(post.Id.ToString()).SendAsync("JoinPostGroup", post.Id);
            }

            return Ok(posts);
        }
        [HttpGet("user/{id}")]
        [SwaggerOperation(Summary = "Get Post By UserId")]
        public IActionResult GetPostByUserId(Guid id)
        {
            var post = postService.GetPostByUserId(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }
    }
}

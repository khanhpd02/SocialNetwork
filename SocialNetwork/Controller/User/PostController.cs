﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.DTO;
using SocialNetwork.DTO.UpdateDTO;
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
        public IActionResult Create([FromForm] PostDTO postDTO)
        {

            if (postDTO == null)
            {
                return BadRequest("Invalid data");
            }
            var createdPost = postService.Create(postDTO);
            return Ok(createdPost);
        }
        [HttpPost("share")]
        [SwaggerOperation(Summary = "Shared a Post")]
        public IActionResult Share([FromForm] ShareDTO sharePostDTO)
        {

            if (sharePostDTO.PostId.ToString().IsNullOrEmpty())
            {
                return BadRequest("Invalid data");
            }
            var sharedPost = postService.SharePost(sharePostDTO);
            return Ok(sharedPost);
        }
        [HttpPut]
        [SwaggerOperation(Summary = "Update a Post")]
        public IActionResult Update([FromForm] PostUpdateDTO postDTO)
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
            var posts = postService.GetAllPostsAndShare();
            return Ok(posts);
        }
        [HttpGet("user/{id}")]
        [SwaggerOperation(Summary = "Get Post By UserId")]
        public IActionResult GetPostByUserId(Guid id)
        {
            var post = postService.GetPostsAndShareByUserId(id);
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);
        }
    }
}

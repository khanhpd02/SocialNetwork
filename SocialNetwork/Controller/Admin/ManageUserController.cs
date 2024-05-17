using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Middlewares;
using SocialNetwork.Model.User;
using SocialNetwork.Service;
using SocialNetwork.Service.Implement;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.Admin
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/admin/user")]
    public class ManageUserController:ControllerBase
    {
        private readonly IAdminService adminService;

        public ManageUserController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        [HttpPost("{id}")]
        [SwaggerOperation(Summary = "get user by Id")]
        public IActionResult getUserById(Guid id)
        {
            var response = adminService.GetUserById(id);

            return Ok(response);
        }
        [HttpPost("{email}")]
        [SwaggerOperation(Summary = "get user by Id")]
        public IActionResult getUserByEmail(string email)
        {

            var response = adminService.GetUserByEmail(email);

            return Ok(response);
        }
        [HttpPost("getall")]
        [SwaggerOperation(Summary = "Get All User")]
        public IActionResult GetAllUser ()
        {
            var response = adminService.GetAllUser();

            return Ok(response);
        }
        [HttpPost("DeleteAllUser")]
        [SwaggerOperation(Summary = "Delete All User")]
        public IActionResult DeleteAllUser()
        {

            var response = adminService.DeleteAllUser();

            return Ok(response);
        }
        [HttpPost("DeleteUserById")]
        [SwaggerOperation(Summary = "Delete User By Id")]
        public IActionResult DeleteUserById(Guid userId)
        {


            var response = adminService.DeleteUserById(userId);

            return Ok(response);
        }
        [HttpPost("DeletePostAdmin")]
        [SwaggerOperation(Summary = "Delete Post By Id")]
        public IActionResult DeletePostById(Guid postId)
        {
            var response = adminService.DeletePostById(postId);

            return Ok(response);
        }
        [HttpPost("GetAllPostsAdmin")]
        [SwaggerOperation(Summary = "Get All Posts")]
        public IActionResult GetAllPosts()
        {
            var response = adminService.GetAllPosts();

            return Ok(response);
        }

    }
}

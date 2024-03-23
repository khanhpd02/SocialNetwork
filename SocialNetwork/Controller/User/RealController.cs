using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using SocialNetwork.Service.Implement;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/real")]
    public class RealController : ControllerBase
    {
        private readonly IRealService realService;
        public RealController(
            IRealService realService) { this.realService = realService; }
        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create Real")]
        public async Task<IActionResult> Create([FromForm] Guid id, IFormFile file)
        {
            var response = await realService.Create(id,file);
            return Ok(response);
        }
    }
}

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
        public async Task<IActionResult> Create([FromForm] MergeImageAndAudioDTO mergeImageAndAudio)
        {
            var response = await realService.MergeImageWithAudio(mergeImageAndAudio);
            return Ok(response);
        }
    }
}

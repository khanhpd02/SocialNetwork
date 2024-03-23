using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using SocialNetwork.Service.Implement;
using SocialNetwork.Socket;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/audio")]
    public class AudioController:ControllerBase
    {
        private readonly IAudioService audioService;

        public AudioController(IAudioService audioService) { this.audioService = audioService; }
        [HttpPost("create")]
        [SwaggerOperation(Summary = "Create Audio")]
        public IActionResult Create([FromForm] AudioDTO dto)
        {
            var response = audioService.Create(dto);
            return Ok(response);
        }
        [HttpGet]
        [SwaggerOperation(Summary = "Get All Audio")]
        public IActionResult GetAll()
        {
            var posts = audioService.GetAllAudio();
            return Ok(posts);
        }
    }
}

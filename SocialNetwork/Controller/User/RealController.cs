using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.ExceptionModel;
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
        [HttpPost("MergeImageWithAudio")]
        [SwaggerOperation(Summary = "MergeImageWithAudio To Real")]
        public async Task<IActionResult> MergeImageWithAudio([FromForm] MergeImageAndAudioDTO mergeImageAndAudio)
        {
            var response = await realService.MergeImageWithAudio(mergeImageAndAudio);
            return Ok(response);
        }
        [HttpPost("MergeVideoWithAudio")]
        [SwaggerOperation(Summary = "MergeVideoWithAudio To Real")]
        public async Task<IActionResult> MergeAudioWithAudio([FromForm] MergVideoAndAudioDTO mergVideoAndAudio)
        {
            var response = await realService.MergeVideoWithAudio(mergVideoAndAudio);
            return Ok(response);
        }
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get Real By Id")]
        public IActionResult GetById(Guid id)
        {
            try
            {
                var result = realService.GetById(id);
                return Ok(result);
            }
            catch (PostNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("user/{userId}")]
        [SwaggerOperation(Summary = "Get Real By UserId")]
        public IActionResult GetRealByUserId(Guid userId)
        {
            try
            {
                var result = realService.GetRealByUserId(userId);
                return Ok(result);
            }
            catch (PostNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get All Real")]
        public IActionResult GetAllReal()
        {
            var result = realService.GetAllReal();
            return Ok(result);
        }
    }
}

﻿using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.Middlewares;
using SocialNetwork.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [TypeFilter(typeof(AuthenticationFilter))]
    [ApiController]
    [Route("api/[controller]")]
    public class FriendController : ControllerBase
    {
        private readonly IFriendService _friendService;

        public FriendController(IFriendService friendService)
        {
            _friendService = friendService;
        }

        [HttpPost("send/{id}")]
        [SwaggerOperation(Summary = "Send Friend Request")]
        public IActionResult SendFriendRequest(Guid id)
        {
            var response = _friendService.SendToFriend(id);
            return Ok(response);
        }

        [HttpPost("accept/{id}")]
        [SwaggerOperation(Summary = "Accept Friend Request")]
        public IActionResult AcceptFriendRequest(Guid id)
        {
            var response = _friendService.AcceptFriend(id);
            return Ok(response);
        }
        [HttpDelete("unfriend/{id}")]
        [SwaggerOperation(Summary = "Un Friend")]
        public IActionResult Unfriend(Guid id)
        {
            var response = _friendService.UnFriend(id);
            return Ok(response);
        }
        [HttpPost("refuseFriend/{id}")]
        [SwaggerOperation(Summary = "Refuse Friend Request")]
        public IActionResult RefuseFriend(Guid id)
        {
            var response = _friendService.RefuseFriend(id);
            return Ok(response);
        }

        [HttpGet("getAll")]
        [SwaggerOperation(Summary = "Get All Friends")]
        public IActionResult GetAllFriends()
        {
            var friends = _friendService.GetAllFriends();
            return Ok(friends);
        }
        [HttpGet("getAllLevels")]
        [SwaggerOperation(Summary = "Get All Friendship Levels")]
        public IActionResult GetAllLevels()
        {
            var levels = _friendService.GetAllLevel();
            return Ok(levels);
        }
        [HttpGet("getAllFriendRequest")]
        [SwaggerOperation(Summary = "Get All Friendship Levels")]
        public IActionResult GetAllFriendRequest()
        {
            var levels = _friendService.GetAllFriendsRequests();
            return Ok(levels);
        }

        [HttpPost("updateFriendLevel")]
        [SwaggerOperation(Summary = "Update Friend's Friendship Level")]
        public IActionResult UpdateFriendLevel([FromBody] FriendDTO dto)
        {
            var response = _friendService.UpdateLevelFriend(dto);
            return Ok(response);
        }
    }

}

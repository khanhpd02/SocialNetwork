﻿using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotifyController : ControllerBase
    {
        private readonly INotifyService _notifyService;

        public NotifyController(INotifyService notifyService)
        {
            _notifyService = notifyService;
        }

        [HttpGet("getAcceptFriendNotifies")]
        [SwaggerOperation(Summary = "Get Notifications for Accepted Friend Requests")]
        public IActionResult GetAcceptFriendNotifies()
        {
            var notifies = _notifyService.GetNotifyAcceptFriendAlongToUser();

            return Ok(notifies);
        }

        [HttpGet("getNotifies")]
        [SwaggerOperation(Summary = "Get Notifications")]
        public IActionResult GetPostNotifies()
        {
            var notifies = _notifyService.GetNotifyAlongToUser();

            return Ok(notifies);
        }
    }

}

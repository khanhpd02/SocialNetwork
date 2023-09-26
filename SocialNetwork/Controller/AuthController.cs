using firstapi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Mail;
using SocialNetwork.Model.User;
using SocialNetwork.Service;

namespace SocialNetwork.Controller
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        private IUserService _userService;
        private IEmailService _emailService;

        public AuthController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("sendpincode")]
        public async Task<IActionResult> sendpincode([FromBody] SendPinEmailModel rsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            var isRegistered = await _userService.SendPinEmail(rsg);
            if (isRegistered)
            {
                return Ok("SendCode successful");
            }
            else
            {
                return BadRequest("Email already exists");
            }
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel rsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            var isRegistered = await _userService.RegisterUser(rsg);
            if (isRegistered)
            {
                return Ok("Registration successful");
            }
            else
            {
                return BadRequest("Email already exists");
            }
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }

            var token = await _userService.Authenticate(loginModel);

            if (token == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(new { Token = token });
        }
        [AllowAnonymous]
        [HttpPost("SendMail")]
        public async Task<IActionResult> SendMail()
        {
            try
            {
                Mailrequest mailrequest = new Mailrequest();
                mailrequest.ToEmail = "duykhanhphan2002@gmail.com";
                mailrequest.Subject = "Welcome to KCT NetWork";
                mailrequest.Body = _emailService.GetHtmlcontent("Cảm ơn đã sử dụng dịch vụ");
                await _emailService.SendEmailAsync(mailrequest);
                return Ok();
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}

using firstapi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
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
        private readonly IRefreshTokenService refreshTokenService;

        private IUserService _userService;
        private IEmailService _emailService;

        public AuthController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("VerifyPin")]
        public IActionResult VerifyPin([FromBody] VerifyPin rsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            string userEmail = Request.Cookies["UserEmail"];
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            var isRegistered = _userService.VerifyPin(rsg, userEmail);
            if (isRegistered != null)
            {
                return Ok("VerifyPin successful");
            }
            else
            {
                return BadRequest("VerifyPin fail");
            }
        }
        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel rsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            var isRegistered = _userService.RegisterUser(rsg);
            if (isRegistered != null)
            {
                Response.Cookies.Append("UserEmail", rsg.Email);

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

            LoginResponse loginResponse =  _userService.Authenticate(loginModel);

            if (loginResponse == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(loginResponse);
        }
        //[HttpPost("login")]
        //public IActionResult Login(LoginModel loginModel)
        //{
        //    var user = _userService.Authenticate(loginModel);
        //    var authResponse = refreshTokenService.GenerateLoginTokens(user);
        //    Response.Cookies.Append("X-Refresh-Token", authResponse.Token?.RefreshToken!, new CookieOptions()
        //    { HttpOnly = true, SameSite = SameSiteMode.None, Expires = DateTime.UtcNow.AddDays(7), Secure = true });

        //    return Ok(new { accessToken = authResponse.Token?.AccessToken, user = authResponse.User });
        //}
        [AllowAnonymous]
        [HttpPost("ReSendMail")]
        public async Task<IActionResult> ReSendMail()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            else
            {
                string userEmail = Request.Cookies["UserEmail"];
                _userService.SendPinEmail(userEmail);
                return Ok("ReSendMail successful");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            

        }

    }
}

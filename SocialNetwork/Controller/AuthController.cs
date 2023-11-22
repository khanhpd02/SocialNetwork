using firstapi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.DTO.Auth;
using SocialNetwork.DTO.Response;
using SocialNetwork.Model.User;
using SocialNetwork.Repository;
using SocialNetwork.Service;
using Swashbuckle.AspNetCore.Annotations;

namespace SocialNetwork.Controller
{

    [Authorize]
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IRefreshTokenService refreshTokenService;

        private IUserService _userService;
        private IEmailService _emailService;
        private readonly IUserRepository userRepository;

        public AuthController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
        }

        [AllowAnonymous]
        [HttpPost("VerifyPin")]
        public async Task<IActionResult> VerifyPinAsync([FromBody] VerifyPin rsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            var isRegistered = await _userService.VerifyPin(rsg);
            if (!isRegistered)
            {
                return Ok("Xác thực thất bại");
            }
            else
            {
                return Ok("Xác thực thành công");
            }


        }
        [AllowAnonymous]
        [HttpPost("VerifyPinForgotPassword")]
        public async Task<IActionResult> VerifyPinForgotPassword([FromBody] VerifyPin rsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            var response =  _userService.VerifyPinForgotPassword(rsg);
            return Ok(response);


        }

        [AllowAnonymous]
        [HttpPost("register")]
        public IActionResult Register(RegisterModel rsg)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            var isRegistered = _userService.RegisterUser(rsg);


            return Ok(isRegistered);


        }
        [AllowAnonymous]
        [HttpPost("changePasswordForgotpassword")]
        public IActionResult changePasswordForgotpassword(LoginModel dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản
            var response = _userService.ChangePasswordForgotPassword(dto);


            return Ok(response);


        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }

            LoginResponse loginResponse = _userService.Authenticate(loginModel);

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
        [SwaggerOperation(Summary = "Gửi mã pin để xác thực tài khoản khi đăng kí")]
        [HttpPost("ReSendPin")]
        public async Task<IActionResult> ReSendMail([FromBody] MailDTO mailDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            else
            {
                String email = mailDTO.Email.ToString();
                _userService.SendPinEmail(email,"VerifyPin");
                return Ok("ReSendMail successful");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản


        }
        [AllowAnonymous]
        [SwaggerOperation(Summary = "Gửi mã pin để lấy lại Mật khảu ")]
        [HttpPost("sendPinforgotPassword")]
        public async Task<IActionResult> SenPinForgotPassword([FromBody] MailDTO mailDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid data");
            }
            else
            {   String email=mailDTO.Email.ToString();
                _userService.SendPinForgotPassword(mailDTO);
                return Ok("Send Pin ForgotPassword successful");
            }
            // Gọi AuthService để xử lý việc đăng ký tài khoản


        }


    }
}

namespace SocialNetwork.Service.Implement;

using AutoMapper;
using BCrypt.Net;
using firstapi.Service;
using global::Service.Implement.ObjectMapping;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.DTO.Auth;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Mail;
using SocialNetwork.Model.User;
using System.Text.RegularExpressions;
using WebApi.Helpers;




public class UserService : IUserService
{
    private SocialNetworkContext _context;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private IEmailService _emailService;
    private static string baseToken = "";

    private User _loggedInUser; // Add a field to store the logged-in user
    public Guid UserId { get; set; }
    public string UserEmail { get; set; }

    private readonly IMapper mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile(new MappingProfile());
    }).CreateMapper();
    public UserService(
        SocialNetworkContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings,
        IEmailService emailService,

      IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
        _emailService = emailService;


        UserId = GetLoggedInUserId(httpContextAccessor.HttpContext);
        UserEmail = GetLoggedInUserEmail(httpContextAccessor.HttpContext);


    }
    public Guid GetLoggedInUserId(HttpContext httpContext)
    {
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var userId = _jwtUtils.ValidateJwtToken(token);

            if (userId.HasValue)
            {
                return userId.Value;
            }
        }

        return Guid.Empty; // No valid token or user not found
    }
    public string GetLoggedInUserEmail(HttpContext httpContext)
    {
        var token = httpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var userEmail = _jwtUtils.ValidateJwtTokenEmail(token);

            if (userEmail != null)
            {
                return userEmail;
            }
        }

        return null; // No valid token or user not found
    }

    public AppResponse RegisterUser(RegisterModel dto)
    {
        if (dto.Email.IsNullOrEmpty())
        {
            throw new BadRequestException("Email field cannot be empty");
        }
        //if (_context.Users.Any(u => u.Email == dto.Email))
        //{
        //    throw new BadRequestException("Email is exist");

        //}
        var emailUSER = _context.Users.Where(x => x.Email == dto.Email).FirstOrDefault();

        if (!IsValidEmail(dto.Email))
        {
            throw new BadRequestException("Email format is not valid");
        }
        if (dto.Password.IsNullOrEmpty())
        {
            throw new BadRequestException("Password field cannot be empty");
        }
        if (emailUSER != null && emailUSER.IsDeleted == false)
        {
            throw new BadRequestException("Email is exist");

        }
        else if (emailUSER != null && emailUSER.IsDeleted == true)
        {
            emailUSER.Password = HashPassword(dto.Password);
            emailUSER.IsDeleted = false;
            _context.Users.Add(emailUSER);
            _context.SaveChanges();

            try
            {
                ///  _generalService.Email = dto.Email;

                SendPinEmail(dto.Email, "VerifyPin");
                return new AppResponse { message = "Gửi mã pin thành công" };
            }
            catch (Exception)
            {

                throw;
            }

        }
        else
        {
            dto.Password = HashPassword(dto.Password);

            User entity = mapper.Map<User>(dto);
            entity.IsDeleted = true;
            _context.Users.Add(entity);
            _context.SaveChanges();
            dto.Password = "";
            var users = _context.Users.Where(u => u.Email == dto.Email).FirstOrDefault();
            users.CreateBy = users.Id;
            _context.Users.Update(users);
            _context.SaveChanges();
            string role = "User";
            var roles = _context.Roles.Where(r => r.RoleName == role).FirstOrDefault();
            UserRole userRole = new UserRole
            {
                UserId = users.Id,
                RoleId = roles.Id,
                IsDeleted = false
            };
            _context.UserRoles.Add(userRole);
            _context.SaveChanges();



            try
            {
                // _generalService.Email = dto.Email;
                SendPinEmail(dto.Email, "VerifyPin");
                return new AppResponse { message = "Gửi mã pin thành công" };
            }
            catch (Exception)
            {

                throw;
            }
        }
        return new AppResponse { message = "Gửi mã pin thất bại" };
        // Gửi mã PIN


    }

    public void SendPinEmail(String Email, String content)
    {


        // Loại bỏ các mã PIN cũ
        var duplicatePinCode = _context.PinCodes.Where(u => u.Email == Email && u.Content == content).ToList();
        foreach (var pincode in duplicatePinCode)
        {
            _context.PinCodes.Remove(pincode);
        }
        _context.SaveChanges();

        // Tạo mã PIN mới và lưu vào cơ sở dữ liệu
        PinCode pin1 = new PinCode
        {
            Email = Email,
            Pin = RandomPIN(),
            CreateDate = DateTime.Now,
            ExpiredTime = DateTime.Now.AddMinutes(3),
            IsDeleted = false,
            Content = content
        };
        _context.PinCodes.Add(pin1);
        _context.SaveChanges();

        // Gửi mã PIN qua email
        try
        {
            Mailrequest mailrequest = new Mailrequest();
            mailrequest.ToEmail = pin1.Email;
            mailrequest.Subject = "Mã Pin " + content;
            mailrequest.Body = _emailService.GetHtmlcontent("Mã pin của bạn là: " + pin1.Pin);
            _emailService.SendEmailAsync(mailrequest).Wait(); // Đợi cho đến khi gửi xong email
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> VerifyPin(VerifyPin VerifyPin)
    {
        var pin = _context.PinCodes.Where(x => x.IsDeleted == false && x.Email == VerifyPin.Email && x.Content == "VerifyPin").FirstOrDefault();
        var user = _context.Users.Where(x => x.Email == VerifyPin.Email).FirstOrDefault();

        if (pin != null)
        {
            if (pin.Pin == VerifyPin.Pin && pin.ExpiredTime >= DateTime.Now)
            {
                user.IsDeleted = false;
                pin.IsDeleted = true;
                _context.Users.Update(user);
                _context.PinCodes.Update(pin);
                _context.SaveChanges();
                return true;
            }
            else
            {
                _context.Users.Remove(user);
                _context.PinCodes.Update(pin);
                _context.SaveChanges();

                throw new BadRequestException("Mã pin sai hoặc hết hạn");

            }
        }
        else
        {
            return false;
        }
    }
    public string HashPassword(string password)
    {
        return BCrypt.EnhancedHashPassword(password, HashType.SHA256);
    }
    public LoginResponse Authenticate(LoginModel loginModel)
    {
        if (loginModel.Email.IsNullOrEmpty())
        {
            throw new BadRequestException("Email field cannot be empty");
        }
        if (!IsValidEmail(loginModel.Email))
        {
            throw new BadRequestException("Email format is not valid");
        }
        if (loginModel.Password.IsNullOrEmpty())
        {
            throw new BadRequestException("Password field cannot be empty");
        }

        var user = _context.Users.SingleOrDefault(u => u.Email == loginModel.Email);

        if (user == null || !VerifyPassword(loginModel.Password, user.Password) || user.IsDeleted == true)
        {
            throw new BadRequestException("Tài khoản hoặc mật khẩu không đúng");

        }
        //lấy role
        List<UserRole> userRole = _context.UserRoles.Where(u => u.UserId == user.Id).ToList();
        List<string> roles = new List<string>();
        foreach (var UserRole in userRole)
        {
            var role = _context.Roles.Where(u => u.Id == UserRole.RoleId).FirstOrDefault();
            roles.Add(role.RoleName.ToString());
        }

        LoginDataResponse loginDataResponse = new LoginDataResponse { Id = user.Id.ToString(), Email = user.Email, JwtToken = _jwtUtils.GenerateJwtToken(user), Role = roles };

        LoginResponse loginResponse = new LoginResponse { Success = true, Code = 1, Data = loginDataResponse, Message = "Đăng nhập thành Công" };

        return loginResponse;
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.EnhancedVerify(password, hashedPassword, HashType.SHA256);
    }
    private string RandomPIN()
    {
        Random random = new Random();
        string sixDigitNumber = string.Empty;

        for (int i = 0; i < 6; i++)
        {
            int randomNumber = random.Next(10); // Tạo một số nguyên ngẫu nhiên từ 0 đến 9
            sixDigitNumber += randomNumber.ToString(); // Thêm số ngẫu nhiên vào chuỗi
        }

        return sixDigitNumber;
    }
    private bool IsValidEmail(string email)
    {
        // Sử dụng biểu thức chính quy để kiểm tra định dạng email
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

    public AppResponse SendPinForgotPassword(MailDTO mailDTO)
    {
        if (mailDTO.Email.IsNullOrEmpty())
        {
            throw new BadRequestException("Email field cannot be empty");
        }

        var emailUSER = _context.Users.Where(x => x.Email == mailDTO.Email).FirstOrDefault();

        if (!IsValidEmail(mailDTO.Email))
        {
            throw new BadRequestException("Email format is not valid");
        }
        if (emailUSER != null && emailUSER.IsDeleted == false)
        {
            SendPinEmail(mailDTO.Email, "ForgotPassword");
            return new AppResponse { message = "Send Pin ForgotPassword successful", success = true };

        }
        else
        {
            throw new BadRequestException("Email chưa được đăng kí");

        }

    }

    public AppResponse VerifyPinForgotPassword(VerifyPin VerifyPin)
    {
        var pin = _context.PinCodes.Where(x => x.IsDeleted == false && x.Email == VerifyPin.Email && x.Content == "ForgotPassword").FirstOrDefault();
        var user = _context.Users.Where(x => x.Email == VerifyPin.Email).FirstOrDefault();

        if (pin != null)
        {
            if (pin.Pin == VerifyPin.Pin && pin.ExpiredTime >= DateTime.Now)
            {
                pin.IsDeleted = true;
                pin.UpdateDate = DateTime.Now;
                pin.Content = "ForgotPasswordVerified";
                _context.PinCodes.Update(pin);
                _context.SaveChanges();
                return new AppResponse { message = "Xác thực thành công", success = true };
            }
            else
            {
                throw new BadRequestException("Mã pin sai hoặc hết hạn");

            }
        }
        else
        {
            throw new BadRequestException("Verify Fail!");
        }
    }

    public AppResponse ChangePasswordForgotPassword(LoginModel loginModel)
    {
        var pin = _context.PinCodes.Where(x => x.IsDeleted == true && x.Email == loginModel.Email && x.Content == "ForgotPasswordVerified").FirstOrDefault();
        var user = _context.Users.Where(x => x.Email == loginModel.Email).FirstOrDefault();

        if (pin != null)
        {
            user.Password = HashPassword(loginModel.Password);
            pin.UpdateDate = DateTime.Now;
            pin.Content = "ForgotPasswordChangedDone";
            _context.Users.Update(user);
            _context.PinCodes.Update(pin);
            _context.SaveChanges();
            return new AppResponse { message = "Đổi mật khẩu thành công", success = true };

        }
        else
        {
            throw new BadRequestException("Change Password for Forgot Password Fail");
        }
    }

}


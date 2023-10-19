namespace SocialNetwork.Service.Implement;

using AutoMapper;
using BCrypt.Net;
using firstapi.Service;
using global::Service.Implement.ObjectMapping;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.DTO;
using SocialNetwork.DTO.Response;
using SocialNetwork.Entity;
using SocialNetwork.ExceptionModel;
using SocialNetwork.Mail;
using SocialNetwork.Model.User;
using SocialNetwork.Repository;
using System.Text.RegularExpressions;
using WebApi.Helpers;




public class UserService : IUserService
{
    private SocialNetworkContext _context;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private IEmailService _emailService;
    private static string baseToken = "";
    private readonly IUserRepository userRepository;
    private readonly IRoleRepository roleRepository;
    private readonly IUserRoleRepository userRoleRepository;
    private readonly IPinCodeRepository pinCodeRepository;

    private readonly IMapper mapper = new MapperConfiguration(cfg =>
    {
        cfg.AddProfile(new MappingProfile());
    }).CreateMapper();
    public UserService(
        SocialNetworkContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings,
        IEmailService emailService,
        IUserRepository userRepository, IRoleRepository roleRepository,
     IUserRoleRepository userRoleRepository, IPinCodeRepository pinCodeRepository)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
        _emailService = emailService;
        this.userRepository = userRepository;
        this.roleRepository = roleRepository;
        this.userRoleRepository = userRoleRepository;
        this.pinCodeRepository = pinCodeRepository;
    }

    public RegisterModel RegisterUser(RegisterModel dto)
    {
        if (dto.Email.IsNullOrEmpty())
        {
            throw new BadRequestException("Email field cannot be empty");
        }
        if (_context.Users.Any(u => u.Email == dto.Email))
        {
            throw new BadRequestException("Email is exist");

        }
        if (!IsValidEmail(dto.Email))
        {
            throw new BadRequestException("Email format is not valid");
        }
        if (dto.Password.IsNullOrEmpty())
        {
            throw new BadRequestException("Password field cannot be empty");
        }

        dto.Password = HashPassword(dto.Password);

        User entity = mapper.Map<User>(dto);
        userRepository.CreateIsTemp(entity);
        userRepository.Save();
        dto.Password = "";
        var users = userRepository.FindByCondition(u => u.Email == dto.Email).FirstOrDefault();
        string role = "User";
        var roles = roleRepository.FindByCondition(r => r.RoleName == role).FirstOrDefault();
        UserRole userRole = new UserRole
        {
            UserId = users.Id,
            RoleId = roles.Id
        };
        userRoleRepository.Create(userRole);
        userRoleRepository.Save();

        try
        {
            SendPinEmail(dto.Email);

        }
        catch (Exception)
        {

            throw;
        }
        // Gửi mã PIN

        return dto;
    }

    public void SendPinEmail(string email)
    {


        // Loại bỏ các mã PIN cũ
        var duplicatePinCode = _context.PinCodes.Where(u => u.Email == email).ToList();
        foreach (var pincode in duplicatePinCode)
        {
            _context.PinCodes.Remove(pincode);
        }
        _context.SaveChanges();

        // Tạo mã PIN mới và lưu vào cơ sở dữ liệu
        PinCode pin1 = new PinCode
        {
            Email = email,
            Pin = RandomPIN(),
            CreateDate = DateTime.Now,
            ExpiredTime = DateTime.Now.AddMinutes(3),
            IsDeleted = false
        };
        _context.PinCodes.Add(pin1);
        _context.SaveChanges();

        // Gửi mã PIN qua email
        try
        {
            Mailrequest mailrequest = new Mailrequest();
            mailrequest.ToEmail = pin1.Email;
            mailrequest.Subject = "Mã Pin Xác Nhận";
            mailrequest.Body = _emailService.GetHtmlcontent("Mã pin của bạn là: " + pin1.Pin);
            _emailService.SendEmailAsync(mailrequest).Wait(); // Đợi cho đến khi gửi xong email
        }
        catch (Exception)
        {
            throw;
        }
    }
    public async Task<bool> VerifyPin(VerifyPin VerifyPin, string email)
    {
        var pin = pinCodeRepository.FindByCondition(x => x.IsDeleted == false && x.Email == email).FirstOrDefault();
        var user = userRepository.FindByCondition(x => x.Email == email).FirstOrDefault();

        if (pin != null)
        {
            if (pin.Pin == VerifyPin.Pin && pin.ExpiredTime >= DateTime.Now)
            {
                user.IsDeleted = false;
                pin.IsDeleted = true;
                userRepository.Update(user);
                pinCodeRepository.Update(pin);
                userRepository.Save();
                pinCodeRepository.Save();
                return true;
            }
            else
            {
                userRepository.Delete(user);
                //pin.IsDeleted = true;
                userRepository.Update(user);
                pinCodeRepository.Update(pin);
                userRepository.Save();
                pinCodeRepository.Save();
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

        if (user == null || !VerifyPassword(loginModel.Password, user.Password)|| user.IsDeleted==true)
        {
            throw new BadRequestException("Tài khoản hoặc mật khẩu không đúng");
     
        }
        //lấy role
        List<UserRole> userRole = userRoleRepository.FindByConditionWithTracking(u => u.UserId == user.Id);
        List<string> roles = new List<string>();
        foreach (var UserRole in userRole)
        {
            var role = roleRepository.FindByCondition(u => u.Id == UserRole.RoleId).FirstOrDefault();
            roles.Add(role.RoleName.ToString());
        }
        //
        LoginDataResponse loginDataResponse = new LoginDataResponse { Id = user.Id.ToString(),Email=user.Email,JwtToken= _jwtUtils.GenerateJwtToken(user),Role=roles };
        
        LoginResponse loginResponse = new LoginResponse {Success=true,Code=1,Data=loginDataResponse,Message="Đăng nhập thành Công"};
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


}


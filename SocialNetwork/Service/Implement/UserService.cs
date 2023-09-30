﻿namespace SocialNetwork.Service.Implement;

using BCrypt.Net;
using firstapi.Service;
using Microsoft.Extensions.Options;
using SocialNetwork.Entity;
using SocialNetwork.Mail;
using SocialNetwork.Model.User;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;
using WebApi.Helpers;



public class UserService : IUserService
{
    private SocialNetworkContext _context;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private IEmailService _emailService;
    private static string baseToken = "";

    public UserService(
        SocialNetworkContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings,
        IEmailService emailService)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
        _emailService = emailService;
    }

    public async Task<bool> SendPinEmail(SendPinEmailModel rsg)
    {
        // Kiểm tra xem email đã tồn tại hay chưa
        if (_context.Users.Any(u => u.Email == rsg.Email))
        {
            return false;
        }
        // Kiểm tra xem email có đúng cấu trúc hay không
        if (!IsValidEmail(rsg.Email))
        {
            throw new AppException("Invalid email");
        }
        //
        var duplicatePinCode = _context.PinCodes.Where(u => u.Email == rsg.Email).ToList();
        foreach (var pincode in duplicatePinCode)
        {
            _context.PinCodes.Remove(pincode);
        }
        _context.SaveChanges();

        //string uniqueId = NanoidDotNet.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 10);
        PinCode pin1 = new PinCode { Email = rsg.Email, Pin = RandomPIN(), CreateDate = DateTime.Now, ExpiredTime = DateTime.Now.AddMinutes(3), IsDeleted = false };
        //
        _context.PinCodes.Add(pin1);
        _context.SaveChanges();
        //send code email
        try
        {

            Mailrequest mailrequest = new Mailrequest();
            mailrequest.ToEmail = pin1.Email;
            mailrequest.Subject = "Mã Pin Xác Nhận";
            mailrequest.Body = _emailService.GetHtmlcontent("Mã pin của bạn là: " + pin1.Pin);
            await _emailService.SendEmailAsync(mailrequest);

        }
        catch (Exception)
        {
            throw;
        }
        return true;
    }
    public async Task<bool> RegisterUser(RegisterModel rsg)
    {
        // Kiểm tra xem email đã tồn tại hay chưa
        if (_context.Users.Any(u => u.Email == rsg.Email))
        {
            return false;
        }
        // Kiểm tra xem email có đúng cấu trúc hay không
        if (!IsValidEmail(rsg.Email))
        {
            throw new AppException("Invalid email");
        }
        //
        using (var transaction =_context.Database.BeginTransaction()) {
            try
            {
                var firstPin = _context.PinCodes.First(user => user.Email == rsg.Email);
                if (firstPin != null)
                {
                    if (firstPin.Pin == rsg.Pin && firstPin.ExpiredTime >= DateTime.Now)
                    {
                        firstPin.IsDeleted = true;
                        // Thêm người dùng mới vào danh sách
                        //string uniqueId = NanoidDotNet.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 10);
                        _context.Users.Add(new User { Email = rsg.Email, Password = BCrypt.HashPassword(rsg.Password), Status = true, CreateDate = DateTime.Now, UpdateDate = DateTime.Now });
                        _context.SaveChanges();
                        User users = _context.Users.SingleOrDefault(u => u.Email == rsg.Email);
                        string role = "User";
                        Role roles = _context.Roles.SingleOrDefault(r => r.RoleName == role);
                        _context.UserRoles.Add(new UserRole { UserId = users.Id, RoleId = roles.Id, CreateDate = DateTime.Now, IsDeleted = false });
                        _context.SaveChanges();
                        
                    }
                    else
                    {
                        throw new AppException("Code sai hoặc hết hạn");
                    }
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new AppException("Đăng kí bị hủy bỏ. Lỗi: " + ex.Message);
            }
        }
            

        return true;
    }
    public async Task<string> Authenticate(LoginModel loginModel)
    {
        var user = _context.Users.SingleOrDefault(u => u.Email == loginModel.Email);

        if (user == null || !BCrypt.Verify(loginModel.Password, user.Password))
        {
            return null;
        }

        return _jwtUtils.GenerateJwtToken(user);
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

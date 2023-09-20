namespace WebApi.Services;

using BCrypt.Net;
using Microsoft.Extensions.Options;
using WebApi.Helpers;
using WebApi.Authorization;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

using SocialNetwork.Model.User;
using SocialNetwork.Entity;

public interface IUserService
{
    /*AuthenticateResponse Authenticate(AuthenticateRequest model);
    AuthenticateResponse RefreshToken(string token);
*/
    Task<bool> RegisterUser(RegisterModel rsg);
  /*  void RevokeToken(string token);
    IEnumerable<User> GetAll();
    User GetById(int id);

    void RemoveAllRefreshTokens(string baseToken);*/
}

public class UserService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    private static string baseToken="";

    public UserService(
        DataContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings)
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
    }

    public async Task<bool> RegisterUser(RegisterModel rsg)
    {
        // Kiểm tra xem email đã tồn tại hay chưa
        if (_context.User.Any(u => u.Email == rsg.Email))
        {
            return false;
        }
        // Kiểm tra xem email có đúng cấu trúc hay không
        if (!IsValidEmail(rsg.Email))
        {
            throw new AppException("Invalid email");
        }

        string uniqueId = NanoidDotNet.Nanoid.Generate("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789", 10);
        // Thêm người dùng mới vào danh sách
        _context.User.Add(new User { Id = uniqueId, Email = rsg.Email, Password = BCrypt.HashPassword(rsg.Password),Status=true,CreateDate=DateTime.Now,UpdateDate=DateTime.Now });
        _context.SaveChanges();

        return true;
    }
    private bool IsValidEmail(string email)
    {
        // Sử dụng biểu thức chính quy để kiểm tra định dạng email
        string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, pattern);
    }

}

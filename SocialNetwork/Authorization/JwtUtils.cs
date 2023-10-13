using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using WebApi.Helpers;
using NanoidDotNet;
using System.Reflection.Emit;
using SocialNetwork.Entity;
using SocialNetwork.Repository;

public interface IJwtUtils
{
    string GenerateJwtToken(User user);
    int? ValidateJwtToken(string token);
    //RefreshToken GenerateRefreshToken();
}

public class JwtUtils : IJwtUtils
{
    private SocialNetworkContext _context;

    private readonly AppSettings _appSettings;
    private readonly IRoleRepository roleRepository;
    private readonly IUserRoleRepository userRoleRepository;

    public JwtUtils(
        SocialNetworkContext context, IRoleRepository roleRepository,
     IUserRoleRepository userRoleRepository,
        IOptions<AppSettings> appSettings)
    {
        _context = context;
        _appSettings = appSettings.Value;
        this.roleRepository = roleRepository;
        this.userRoleRepository = userRoleRepository;
    }

    public string GenerateJwtToken(User user)
    {
        // generate token that is valid for 15 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        List<UserRole> userRole = userRoleRepository.FindByConditionWithTracking(u => u.UserId == user.Id);
        List<string> roles = new List<string>();   
        foreach (var UserRole in userRole)
        {
            var role = roleRepository.FindByCondition(u => u.Id == UserRole.RoleId).FirstOrDefault();
            roles.Add(role.RoleName.ToString());
        }
        var roleClaims = roles.Select(role => new Claim("role", role)).ToList();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()),
             }.Concat(roleClaims)
            ),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public int? ValidateJwtToken(string token)
    {
        if (token == null)
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            // return user id from JWT token if validation successful
            return userId;
        }
        catch
        {
            // return null if validation fails
            return null;
        }
    }

   /* public RefreshToken GenerateRefreshToken()
    {
        var refreshToken = new RefreshToken
        {
            Token = GenerateUniqueToken(),
            Expires = DateTime.UtcNow.AddDays(7),
            Created = DateTime.UtcNow,
        };

        return refreshToken;
    }*/

    private string GenerateUniqueToken()
    {
        const int length = 10; // Độ dài token cần tạo

        var alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var uniqueToken = Nanoid.Generate(alphabet, length);
/*
        // Kiểm tra token có duy nhất hay không bằng cách kiểm tra trong cơ sở dữ liệu
        var tokenIsUnique = !_context.Users.Any(u => u.RefreshTokens.Any(t => t.Token == uniqueToken));

        if (!tokenIsUnique)
            return GenerateUniqueToken(); // Nếu không duy nhất, thử tạo token mới lần nữa*/

        return uniqueToken;
    }
}

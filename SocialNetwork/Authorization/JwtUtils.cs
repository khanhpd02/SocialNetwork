using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NanoidDotNet;
using SocialNetwork.Entity;
using SocialNetwork.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApi.Helpers;

public interface IJwtUtils
{
    string GenerateJwtToken(User user);
    Guid? ValidateJwtToken(string token);
    string? ValidateJwtTokenEmail(string token);

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
    //public string GenerateJwtToken(User user)
    //{
    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

    //    List<UserRole> userRoles = userRoleRepository.FindByConditionWithTracking(u => u.UserId == user.Id);
    //    List<string> roles = new List<string>();

    //    foreach (var userRole in userRoles)
    //    {
    //        var role = roleRepository.FindByCondition(u => u.Id == userRole.RoleId).FirstOrDefault();
    //        roles.Add(role.RoleName.ToString());
    //    }

    //    var roleClaims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();

    //    // Add claim for email
    //    var emailClaim = new Claim(ClaimTypes.Email, user.Email);
    //    var idClaim = new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());

    //    var allClaims = new List<Claim>
    //{
    //    emailClaim,
    //    idClaim
    //}.Concat(roleClaims);

    //    var tokenDescriptor = new SecurityTokenDescriptor
    //    {
    //        Subject = new ClaimsIdentity(allClaims),
    //        Expires = DateTime.UtcNow.AddDays(3),
    //        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    //    };

    //    var token = tokenHandler.CreateToken(tokenDescriptor);
    //    return tokenHandler.WriteToken(token);
    //}

    public string GenerateJwtToken(User user)
    {
        // generate token that is valid for 15 minutes
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        List<UserRole> userRoles = userRoleRepository.FindByConditionWithTracking(u => u.UserId == user.Id);
        List<string> roles = new List<string>();

        foreach (var userRole in userRoles)
        {
            var role = roleRepository.FindByCondition(u => u.Id == userRole.RoleId).FirstOrDefault();
            roles.Add(role.RoleName.ToString());
        }

        var roleClaims = roles.Select(role => new Claim("role", role)).ToList();

        // Thêm claim về email vào danh sách các claims
        var emailClaim = new Claim("email", user.Email);
        var allClaims = new List<Claim>
    {
        new Claim("id", user.Id.ToString()),
        emailClaim
    }.Concat(roleClaims);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(allClaims),
            Expires = DateTime.UtcNow.AddDays(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }


    public Guid? ValidateJwtToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            return userId;
        }
        catch (Exception)
        {
            // Log or handle the exception
            return null;
        }
    }
    public string? ValidateJwtTokenEmail(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

        try
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userEmailClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "email");

            if (userEmailClaim != null)
            {
                var userEmail = userEmailClaim.Value;
                return userEmail;
            }

            return null;
        }
        catch (Exception)
        {
            // Log or handle the exception
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

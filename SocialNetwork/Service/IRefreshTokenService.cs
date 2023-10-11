using SocialNetwork.DTO;
using SocialNetwork.Entity;

namespace SocialNetwork.Service
{
    public interface IRefreshTokenService
    {
        List<RefreshTokenDTO> GetAll();

        RefreshTokenDTO GetByUserId(string id);

        bool IsActivate(RefreshTokenDTO dto);

        RefreshTokenDTO Create(RefreshTokenDTO dto);

        void Delete(string id);

        AuthResponse GenerateLoginTokens(User user);

        AuthResponse RefreshToken(string token);
        void DeleteRefreshTokenExpired();

        void DeleteTokenWhenLogout(string refreshTokenId);
    }
}

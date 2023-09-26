using SocialNetwork.Model.User;

namespace SocialNetwork.Service
{
    public interface IUserService
    {
        Task<bool> SendPinEmail(SendPinEmailModel rsg);
        Task<bool> RegisterUser(RegisterModel rsg);
        Task<string> Authenticate(LoginModel loginModel);
    }
}

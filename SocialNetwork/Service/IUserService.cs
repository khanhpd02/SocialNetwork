using SocialNetwork.DTO.Response;
using SocialNetwork.Model.User;

namespace SocialNetwork.Service
{
    public interface IUserService
    {
        //Task<bool> SendPinEmail(SendPinEmailModel rsg);
        Task<bool> VerifyPin(VerifyPin VerifyPin, string email);
        AppResponse RegisterUser(RegisterModel rsg);
        LoginResponse Authenticate(LoginModel loginModel);
        void SendPinEmail(string email);
    }
}

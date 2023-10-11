using SocialNetwork.Model.User;

namespace SocialNetwork.Service
{
    public interface IUserService
    {
        //Task<bool> SendPinEmail(SendPinEmailModel rsg);
        VerifyPin VerifyPin(VerifyPin VerifyPin, string email);
        RegisterModel RegisterUser(RegisterModel rsg);
        Task<string> Authenticate(LoginModel loginModel);
        void SendPinEmail(string email);
    }
}

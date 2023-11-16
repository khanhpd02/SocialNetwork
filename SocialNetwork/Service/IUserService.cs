using SocialNetwork.DTO.Response;
using SocialNetwork.Model.User;

namespace SocialNetwork.Service
{
    public interface IUserService
    {
        Guid UserId { get; set; }
        string UserEmail { get; set; }
        //Task<bool> SendPinEmail(SendPinEmailModel rsg);
        Task<bool> VerifyPin(VerifyPin VerifyPin);
        AppResponse RegisterUser(RegisterModel rsg);
        LoginResponse Authenticate(LoginModel loginModel);
        void SendPinEmail();
    }
}

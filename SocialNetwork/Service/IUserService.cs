using SocialNetwork.DTO.Auth;
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
        AppResponse VerifyPinForgotPassword(VerifyPin VerifyPin);
        AppResponse ChangePasswordForgotPassword(LoginModel loginModel);
        AppResponse RegisterUser(RegisterModel rsg);
        AppResponse SendPinForgotPassword(MailDTO mailDTO);
        LoginResponse Authenticate(LoginModel loginModel);
        void SendPinEmail(String Email, String content);
    }
}

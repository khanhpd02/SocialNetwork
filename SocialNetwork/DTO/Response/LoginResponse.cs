using System.Text.Json.Serialization;

namespace SocialNetwork.DTO.Response
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object FirebaseData { get; set; }

        // [JsonIgnore] // refresh token is returned in http only cookie
        //public string Token { get; set; }
        /*public LoginResponse(bool success, int code, string message, Object data *//*string refreshToken*//*)
        {
            Success = success;
            Code = code;
            Data = data;
            //Token = refreshToken;
            Message = message;
        }*/
    }
}

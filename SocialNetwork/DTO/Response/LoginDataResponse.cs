namespace SocialNetwork.DTO.Response
{
    public class LoginDataResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public List<string> Role { get; set; }
        //public string vendorId { get; set; }
        public string JwtToken { get; set; }
        public bool HasInfor { get; set; }
    }
}

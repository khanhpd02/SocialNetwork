namespace SocialNetwork.Service.Implement
{
    public class GeneralService : IGeneralService
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }
        public string CloudinaryUrl { get; set; }
    }
}

namespace SocialNetwork.Service
{
    public interface IGeneralService
    {
        Guid UserId { get; set; }
        public string UserName { get; set; }
        string Email { get; set; }
        string CloudinaryUrl { get; set; }
    }
}

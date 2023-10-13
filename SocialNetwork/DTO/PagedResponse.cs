namespace SocialNetwork.DTO
{
    public class PagedResponse : ReponseModel
    {
        public Pagable pagable { get; set; } = null!;
    }
}

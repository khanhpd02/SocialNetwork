namespace SocialNetwork.DTO
{
    public class Pagable
    {
        public int offset { get; set; }
        public int pageSize { get; set; }
        public int? total { get; set; }
    }
}

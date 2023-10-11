namespace SocialNetwork.DTO
{
    public class RefreshTokenDTO
    {
        public Guid? Id { get; set; }

        public string UserId { get; set; } = null!;

        public string? Value { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}

namespace SocialNetwork.Entity
{
    public partial class RefreshToken
    {
        public Guid Id { get; set; }

        public string? FamilyId { get; set; }

        public Guid? UserId { get; set; }

        public bool? IsActivated { get; set; }

        public DateTime? ExpirationDate { get; set; }

        public virtual User? User { get; set; }
    }
}

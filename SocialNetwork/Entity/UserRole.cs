namespace SocialNetwork.Entity
{
    public class UserRole
    {
        public string UserId { get; set; }
        public string RoleId { get; set; }
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}

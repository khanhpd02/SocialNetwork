namespace SocialNetwork.Entity
{
    public class Role
    {
        public string Id { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
        
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}

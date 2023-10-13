namespace SocialNetwork.DTO
{
    public class UserDTO
    {
        public Guid? Id { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }

        public bool? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public Guid? CreateBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public Guid? UpdateBy { get; set; }

        public bool IsDeleted { get; set; }



    }
}

namespace SocialNetwork.Entity
{
    using System.Text.Json.Serialization;

    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
        public string InforId { get; set; }

        [JsonIgnore]
        public Boolean Status { get; set; }
        
        public DateTime CreateDate { get; set; }
        public string CreateBy { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdateDate { get; set; }
        public Boolean IsDeleted { get; set; }



        /*public User() { }
        public User(string userId, string email, string password, string fullName)
        {
            UserId = userId;
            FullName = fullName;
            Email = email;
            PasswordHash = password; // Hãy thay HashPassword bằng phương thức băm mật khẩu thực tế của bạn
        }*/
    }
}

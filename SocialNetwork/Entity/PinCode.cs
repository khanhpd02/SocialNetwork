namespace SocialNetwork.Entity
{
    public class PinCode
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Pin { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime ExpiredTime { get; set; }
        public Boolean IsDeleted { get; set; }


    }
}

namespace SocialNetwork.DTO;

public class InforDTO
{
    public Guid? Id { get; set; }

    public Guid? UserId { get; set; }

    public string? FullName { get; set; }

    public string? WorkPlace { get; set; }

    public bool? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public IFormFile? File { get; set; }
    public string? Image { get; set; }

    public string? Address { get; set; }

    public string? StatusFriend { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public string LevelFriend { get; set; }

    //public DateTime? CreateDate { get; set; }

    //public Guid? CreateBy { get; set; }

    //public DateTime? UpdateDate { get; set; }

    //public Guid? UpdateBy { get; set; }

    //public bool IsDeleted { get; set; }
}

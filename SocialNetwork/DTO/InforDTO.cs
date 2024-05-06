namespace SocialNetwork.DTO;

public class InforDTO
{
    public object FirebaseData { get; set; }
    public int CountFriend { get; set; }
    public int CountPost { get; set; }
    public Guid? Id { get; set; }

    public Guid? UserId { get; set; }

    public string? FullName { get; set; }

    public string? WorkPlace { get; set; }

    public bool? Gender { get; set; }

    public string? PhoneNumber { get; set; }

    public IFormFile? File { get; set; }
    public IFormFile? FileBackground { get; set; }
    public string? Image { get; set; }

    public string? Address { get; set; }

    public string? StatusFriend { get; set; }

    public DateTime? DateOfBirth { get; set; }
    public string LevelFriend { get; set; }

    public string Provinces { get; set; }

    public string Districts { get; set; }

    public string Wards { get; set; }

    public string Direction { get; set; }
    public string Background { get; set; }

    public string Career { get; set; }

    public string Nickname { get; set; }

    public List<string> RoleName { get; set; }


}

namespace SocialNetwork.DTO;

public class GroupDTO
{
    public Guid? Id { get; set; }

    public string? GroupName { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

}

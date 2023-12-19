namespace SocialNetwork.DTO;

public class CommentDTO
{
    public Guid? Id { get; set; }

    public string? Content { get; set; }

    public Guid? PostId { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }
    public Guid? ParentId { get; set; }
    public string? Image { get; set; }
    public string? FullName { get; set; }

    public List<CommentDTO> ChildrenComment { get; set; }

}

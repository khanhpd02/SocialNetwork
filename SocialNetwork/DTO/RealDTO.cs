using SocialNetwork.Entity;

namespace SocialNetwork.DTO;

public class RealDTO
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public int LevelView { get; set; }
    public List<Video> Videos { get; set; }
    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public int CountLike { get; set; }
    public int CountComment { get; set; }
    public bool islike { get; set; }

}

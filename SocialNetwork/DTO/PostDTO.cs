using SocialNetwork.Entity;

namespace SocialNetwork.DTO;

public class PostDTO
{
    public Guid? Id { get; set; }
    public string? Content { get; set; }
    public string? FullName { get; set; }
    public string? AvatarUrl { get; set; }
    public Guid UserId { get; set; }
    public List<IFormFile?> File { get; set; }

    public DateTime? CreateDate { get; set; }
    public DateTime? CreateDateShare { get; set; }

    public Guid? CreateBy { get; set; }

    public List<Image> Images { get; set; }
    public List<Video> Videos { get; set; }
    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }
    public int CountLike { get; set; }
    public int CountComment { get; set; }
    public bool islike { get; set; }
    public int LevelView { get; set; }
    public int? LevelViewShare { get; set; }
    public string? FullNameShare { get; set; }
    public string? AvatarUrlShare { get; set; }
    public PostDTO()
    {
        Images = new List<Image>();
        Videos = new List<Video>();
        Likes = new List<Like>();
        Comments = new List<Comment>();
    }
    public bool IsDeleted { get; set; }
    public Guid? UserIdSharePost { get; set; }

}

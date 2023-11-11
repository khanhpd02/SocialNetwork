using SocialNetwork.Entity;

namespace SocialNetwork.DTO;

public class PostDTO
{
    public Guid? Id { get; set; }
    public string Content { get; set; }

    public Guid UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public List<Image> Images { get; set; }
    public List<Video> Videos { get; set; }
    public List<Like> Likes { get; set; }
    public List<Comment> Comments { get; set; }
    public int CountLike { get; set; }
    public int CountComment { get; set; }

    public PostDTO()
    {
        Images = new List<Image>();
        Videos = new List<Video>();
        Likes = new List<Like>();
        Comments = new List<Comment>();
    }
}

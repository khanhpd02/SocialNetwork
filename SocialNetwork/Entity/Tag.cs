namespace SocialNetwork.Entity;

public partial class Tag : IEntity
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<TagPost> TagPosts { get; set; } = new List<TagPost>();
}

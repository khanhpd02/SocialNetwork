namespace SocialNetwork.Entity;

public partial class Image : IEntity
{
    public Guid Id { get; set; }

    public string LinkImage { get; set; }

    public Guid? PostId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Post Post { get; set; }
}

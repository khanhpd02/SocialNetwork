namespace SocialNetwork.Entity;

public partial class UserGroupChat : IEntity
{
    public Guid Id { get; set; }

    public Guid? UserId { get; set; }

    public Guid? GroupChatId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual GroupChat GroupChat { get; set; }

    public virtual User User { get; set; }
}

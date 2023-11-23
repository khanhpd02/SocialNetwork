namespace SocialNetwork.Entity;

public partial class GroupChat : IEntity
{
    public Guid Id { get; set; }

    public string GroupName { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<UserGroupChat> UserGroupChats { get; set; } = new List<UserGroupChat>();
}

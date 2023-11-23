namespace SocialNetwork.Entity;

public partial class Role : IEntity
{
    public Guid Id { get; set; }

    public string RoleName { get; set; }

    public string Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

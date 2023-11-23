namespace SocialNetwork.Entity;

public partial class User : IEntity
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }

    public bool? Status { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public string UserName { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Friend> FriendUserAcceptNavigations { get; set; } = new List<Friend>();

    public virtual ICollection<Friend> FriendUserToNavigations { get; set; } = new List<Friend>();

    public virtual ICollection<Infor> Infors { get; set; } = new List<Infor>();

    public virtual ICollection<Like> Likes { get; set; } = new List<Like>();

    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();

    public virtual ICollection<UserGroupChat> UserGroupChats { get; set; } = new List<UserGroupChat>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

namespace SocialNetwork.Entity;

public partial class PinCode : IEntity
{
    public Guid Id { get; set; }

    public string Email { get; set; }

    public string Pin { get; set; }

    public DateTime? CreateDate { get; set; }

    public DateTime? ExpiredTime { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public string Content { get; set; }
}

﻿namespace SocialNetwork.Entity;

public partial class Friend : IEntity
{
    public Guid Id { get; set; }

    public Guid UserTo { get; set; }

    public Guid UserAccept { get; set; }

    public int Level { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual User UserAcceptNavigation { get; set; } = null!;

    public virtual User UserToNavigation { get; set; } = null!;
}

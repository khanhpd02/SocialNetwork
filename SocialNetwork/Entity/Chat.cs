﻿namespace SocialNetwork.Entity;

public partial class Chat : IEntity
{
    public Guid Id { get; set; }

    public int? ConvertationId { get; set; }

    public string? Context { get; set; }

    public bool IsSeen { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }
}

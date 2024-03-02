using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Chat : IEntity
{
    public Guid Id { get; set; }

    public Guid? RoomId { get; set; }

    public Guid? UserFromId { get; set; }

    public Guid? UserToId { get; set; }

    public Guid? ConvertationId { get; set; }

    public string Context { get; set; }

    public bool IsSeen { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }
}

using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class UserRole : IEntity
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public Guid Id { get; set; }

    public virtual Role Role { get; set; }

    public virtual User User { get; set; }
}

﻿using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Role
{
    public Guid Id { get; set; }

    public string? Role1 { get; set; }

    public string? Description { get; set; }

    public DateTime? CreateDate { get; set; }

    public int? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public int? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}

﻿using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class Comment : IEntity
{
    public Guid Id { get; set; }

    public string Content { get; set; }

    public Guid? PostId { get; set; }

    public Guid? UserId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public Guid? ParentId { get; set; }

    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();

    public virtual Comment Parent { get; set; }
}

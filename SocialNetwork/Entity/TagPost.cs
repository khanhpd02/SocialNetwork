﻿using System;
using System.Collections.Generic;

namespace SocialNetwork.Entity;

public partial class TagPost : IEntity
{
    public Guid TagId { get; set; }

    public Guid PostId { get; set; }

    public DateTime? CreateDate { get; set; }

    public Guid? CreateBy { get; set; }

    public DateTime? UpdateDate { get; set; }

    public Guid? UpdateBy { get; set; }

    public bool IsDeleted { get; set; }

    public Guid Id { get; set; }

    public virtual Post Post { get; set; }

    public virtual Tag Tag { get; set; }
}
